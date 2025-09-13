using System.Text.Json;
using System.Text.RegularExpressions;
using UPL.Models;
using Microsoft.Playwright;

namespace UPL.Infrastructure.Services.Video;

public class TikTokVideoProvider : ITikTokVideoProvider
{
    private readonly HttpClient _http;
    private readonly TikTokOptions _options;
    private readonly ILogger<TikTokVideoProvider> _logger;

    public TikTokVideoProvider(HttpClient httpClient, Microsoft.Extensions.Options.IOptions<TikTokOptions> options, ILogger<TikTokVideoProvider> logger)
    {
        _http = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public Task<IReadOnlyList<VideoDto>> GetProfileVideosAsync(string profileUrl, int max = int.MaxValue, CancellationToken ct = default)
        => GetProfileVideosAsync(profileUrl, preferHeadless: false, max, null, ct);

    public async Task<IReadOnlyList<VideoDto>> GetProfileVideosAsync(string profileUrl, bool preferHeadless, int max = int.MaxValue, string? cookieKey = null, CancellationToken ct = default)
    {
        // Normalize URL (keep path, drop query to avoid locale complications)
        var uri = new Uri(profileUrl);
        var baseUrl = $"{uri.Scheme}://{uri.Host}{uri.AbsolutePath.TrimEnd('/')}";

        if (preferHeadless)
        {
            try
            {
                var headlessDirect = await FetchWithPlaywrightAsync(baseUrl, max, cookieKey, ct);
                if (headlessDirect.Count > 0) return headlessDirect;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "TikTok headless forced failed");
            }
        }

        var req = new HttpRequestMessage(HttpMethod.Get, baseUrl);
        req.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0 Safari/537.36");
        req.Headers.Referrer = new Uri("https://www.tiktok.com/");
        req.Headers.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
        req.Headers.AcceptLanguage.ParseAdd("en-US,en;q=0.9,vi;q=0.8");
        var selectedCookie = SelectCookieFor(profileUrl, cookieKey);
        if (!string.IsNullOrWhiteSpace(selectedCookie))
        {
            try { req.Headers.Add("Cookie", selectedCookie); } catch {}
        }

        using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
        resp.EnsureSuccessStatusCode();
        var html = await resp.Content.ReadAsStringAsync(ct);

        // Extract SIGI_STATE JSON
        string? json = null;
        // Pattern 1: script tag with id="SIGI_STATE"
        var m1 = Regex.Match(html, "<script[^>]*id=\"SIGI_STATE\"[^>]*>(.*?)</script>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        if (m1.Success) json = m1.Groups[1].Value;
        if (json == null)
        {
            // Pattern 2: window['SIGI_STATE'] = {...};
            var m2 = Regex.Match(html, "window\\s*\\[\\'SIGI_STATE\\'\\]\\s*=\\s*(\\{.*?\\})\\s*;", RegexOptions.Singleline);
            if (m2.Success) json = m2.Groups[1].Value;
        }
        if (json == null)
        {
            // Pattern 3: window.SIGI_STATE = {...};
            var m3 = Regex.Match(html, "window\\.SIGI_STATE\\s*=\\s*(\\{.*?\\})\\s*;", RegexOptions.Singleline);
            if (m3.Success) json = m3.Groups[1].Value;
        }

        if (string.IsNullOrWhiteSpace(json))
        {
            // Fallback: try mobile host if a username segment exists
            var segs = uri.AbsolutePath.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
            var userSeg = segs.FirstOrDefault(s => s.StartsWith("@"));
            if (userSeg != null)
            {
                var mobileUrl = $"https://m.tiktok.com/{userSeg}";
                _logger.LogInformation("TikTok: primary parse failed, trying mobile URL {url}", mobileUrl);
                var req2 = new HttpRequestMessage(HttpMethod.Get, mobileUrl);
                req2.Headers.UserAgent.ParseAdd("Mozilla/5.0 (iPhone; CPU iPhone OS 15_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/15.0 Mobile/15E148 Safari/604.1");
                req2.Headers.Referrer = new Uri("https://www.tiktok.com/");
                req2.Headers.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
                req2.Headers.AcceptLanguage.ParseAdd("en-US,en;q=0.9,vi;q=0.8");
                if (!string.IsNullOrWhiteSpace(selectedCookie))
                {
                    try { req2.Headers.Add("Cookie", selectedCookie); } catch {}
                }
                using var resp2 = await _http.SendAsync(req2, HttpCompletionOption.ResponseHeadersRead, ct);
                if (resp2.IsSuccessStatusCode)
                {
                    var html2 = await resp2.Content.ReadAsStringAsync(ct);
                    var m1b = Regex.Match(html2, "<script[^>]*id=\"SIGI_STATE\"[^>]*>(.*?)</script>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    if (m1b.Success) json = m1b.Groups[1].Value;
                    if (json == null)
                    {
                        var m2b = Regex.Match(html2, "window\\s*\\[\\'SIGI_STATE\\'\\]\\s*=\\s*(\\{.*?\\})\\s*;", RegexOptions.Singleline);
                        if (m2b.Success) json = m2b.Groups[1].Value;
                    }
                    if (json == null)
                    {
                        var m3b = Regex.Match(html2, "window\\.SIGI_STATE\\s*=\\s*(\\{.*?\\})\\s*;", RegexOptions.Singleline);
                        if (m3b.Success) json = m3b.Groups[1].Value;
                    }
                }
            }
            if (string.IsNullOrWhiteSpace(json))
            {
                _logger.LogWarning("TikTok: SIGI_STATE not found for {url}", baseUrl);
                return Array.Empty<VideoDto>();
            }
        }

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var results = new List<VideoDto>(max);

        // Prefer ItemList->user-post->list of IDs + ItemModule lookup
        string[]? itemIds = null;
        if (root.TryGetProperty("ItemList", out var itemList))
        {
            foreach (var prop in itemList.EnumerateObject())
            {
                if (prop.Name.Contains("user-post", StringComparison.OrdinalIgnoreCase))
                {
                    var listProp = prop.Value.TryGetProperty("list", out var listVal) ? listVal : default;
                    if (listProp.ValueKind == JsonValueKind.Array)
                    {
                        itemIds = listProp.EnumerateArray().Select(e => e.GetString()!).Where(s => !string.IsNullOrEmpty(s)).ToArray();
                        break;
                    }
                }
            }
        }

        // Find fallback username if needed
        string? username = null;
        if (root.TryGetProperty("UserModule", out var userModule) && userModule.TryGetProperty("users", out var usersObj))
        {
            var firstUser = usersObj.EnumerateObject().FirstOrDefault();
            if (firstUser.Value.ValueKind != JsonValueKind.Undefined)
            {
                if (firstUser.Value.TryGetProperty("uniqueId", out var uid) && uid.ValueKind == JsonValueKind.String)
                    username = uid.GetString();
            }
        }

        // Build from ItemModule
        if (root.TryGetProperty("ItemModule", out var itemModule) && itemModule.ValueKind == JsonValueKind.Object)
        {
            IEnumerable<JsonProperty> itemsEnum = itemModule.EnumerateObject();
            if (itemIds != null)
            {
                var set = new HashSet<string>(itemIds);
                itemsEnum = itemsEnum.Where(p => set.Contains(p.Name));
            }

            foreach (var p in itemsEnum)
            {
                var id = p.Name;
                var obj = p.Value;
                var title = obj.TryGetProperty("title", out var t) && t.ValueKind == JsonValueKind.String
                    ? t.GetString() ?? string.Empty
                    : obj.TryGetProperty("desc", out var d) && d.ValueKind == JsonValueKind.String
                        ? d.GetString() ?? string.Empty
                        : string.Empty;

                string? url = null;
                if (obj.TryGetProperty("shareUrl", out var sUrl) && sUrl.ValueKind == JsonValueKind.String)
                {
                    url = sUrl.GetString();
                }
                else
                {
                    var author = obj.TryGetProperty("author", out var a) && a.ValueKind == JsonValueKind.String ? a.GetString() : null;
                    var user = !string.IsNullOrEmpty(author) ? author : username;
                    if (!string.IsNullOrEmpty(user))
                        url = $"https://www.tiktok.com/@{user}/video/{id}";
                }

                if (!string.IsNullOrEmpty(url))
                {
                    results.Add(new VideoDto { Title = title, Url = url });
                    if (results.Count >= max) break;
                }
            }
        }

        // If no results but we have IDs (rare), build minimal URLs
        if (results.Count == 0 && itemIds != null && !string.IsNullOrEmpty(username))
        {
            foreach (var id in itemIds)
            {
                results.Add(new VideoDto { Title = string.Empty, Url = $"https://www.tiktok.com/@{username}/video/{id}" });
                if (results.Count >= max) break;
            }
        }

        if (results.Count > 0)
            return results;

        // Headless fallback with Playwright if HTML parsing yielded nothing
        try
        {
            var headless = await FetchWithPlaywrightAsync(baseUrl, max, cookieKey, ct);
            if (headless.Count > 0) return headless;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "TikTok headless fallback failed");
        }

        return results;
    }

    private async Task<IReadOnlyList<VideoDto>> FetchWithPlaywrightAsync(string url, int max, string? cookieKey, CancellationToken ct)
    {
        var results = new List<VideoDto>(Math.Min(max, 100));
        using var reg = ct.Register(() => { /* no-op; Playwright doesn't accept CT everywhere */});

        using var playwright = await Playwright.CreateAsync();
        await InstallBrowsersIfNeededAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0 Safari/537.36"
        });

        // Apply cookies if provided
        var selectedCookieForPlaywright = SelectCookieFor(url, cookieKey);
        if (!string.IsNullOrWhiteSpace(selectedCookieForPlaywright))
        {
            var cookiePairs = selectedCookieForPlaywright.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var cookies = new List<Cookie>();
            foreach (var pair in cookiePairs)
            {
                var idx = pair.IndexOf('=');
                if (idx <= 0) continue;
                var name = pair.Substring(0, idx).Trim();
                var val = pair.Substring(idx + 1).Trim();
                cookies.Add(new Cookie { Name = name, Value = val, Url = "https://www.tiktok.com/" });
            }
            if (cookies.Count > 0)
            {
                await context.AddCookiesAsync(cookies);
            }
        }

        var page = await context.NewPageAsync();
        await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle, Timeout = 45000 });

        // Dismiss cookie banner if present
        try
        {
            var btn = await page.QuerySelectorAsync("button:has-text('Accept')");
            if (btn != null) await btn.ClickAsync();
        }
        catch { }

        // Try to parse SIGI_STATE from the page first (more reliable)
        try
        {
            var sigi = await page.EvaluateAsync<string>("() => { try { const j = JSON.stringify(window.SIGI_STATE || window['SIGI_STATE']); return j || null; } catch { return null; } }");
            if (!string.IsNullOrEmpty(sigi))
            {
                using var doc = JsonDocument.Parse(sigi);
                var root = doc.RootElement;

                string[]? itemIds = null;
                if (root.TryGetProperty("ItemList", out var itemList))
                {
                    foreach (var prop in itemList.EnumerateObject())
                    {
                        if (prop.Name.Contains("user-post", StringComparison.OrdinalIgnoreCase))
                        {
                            if (prop.Value.TryGetProperty("list", out var listVal) && listVal.ValueKind == JsonValueKind.Array)
                            {
                                itemIds = listVal.EnumerateArray().Select(e => e.GetString()!).Where(s => !string.IsNullOrEmpty(s)).ToArray();
                                break;
                            }
                        }
                    }
                }

                string? username = null;
                if (root.TryGetProperty("UserModule", out var userModule) && userModule.TryGetProperty("users", out var usersObj))
                {
                    var firstUser = usersObj.EnumerateObject().FirstOrDefault();
                    if (firstUser.Value.ValueKind != JsonValueKind.Undefined)
                    {
                        if (firstUser.Value.TryGetProperty("uniqueId", out var uid) && uid.ValueKind == JsonValueKind.String)
                            username = uid.GetString();
                    }
                }

                if (root.TryGetProperty("ItemModule", out var itemModule) && itemModule.ValueKind == JsonValueKind.Object)
                {
                    IEnumerable<JsonProperty> itemsEnum = itemModule.EnumerateObject();
                    if (itemIds != null)
                    {
                        var set = new HashSet<string>(itemIds);
                        itemsEnum = itemsEnum.Where(p => set.Contains(p.Name));
                    }

                    foreach (var p in itemsEnum)
                    {
                        var id = p.Name;
                        var obj = p.Value;
                        var title = obj.TryGetProperty("title", out var t) && t.ValueKind == JsonValueKind.String
                            ? t.GetString() ?? string.Empty
                            : obj.TryGetProperty("desc", out var d) && d.ValueKind == JsonValueKind.String
                                ? d.GetString() ?? string.Empty
                                : string.Empty;
                        string? vurl = null;
                        if (obj.TryGetProperty("shareUrl", out var sUrl) && sUrl.ValueKind == JsonValueKind.String)
                            vurl = sUrl.GetString();
                        else if (!string.IsNullOrEmpty(username))
                            vurl = $"https://www.tiktok.com/@{username}/video/{id}";
                        if (!string.IsNullOrEmpty(vurl))
                        {
                            results.Add(new VideoDto { Title = title, Url = vurl });
                            if (results.Count >= max) return results;
                        }
                    }
                }
            }
        }
        catch { }

        // If still not enough, scroll to load items (adaptive loops)
        int stagnant = 0;
        for (int i = 0; i < 200 && results.Count < max && stagnant < 5; i++)
        {
            await page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight)");
            await page.WaitForTimeoutAsync(900);

            var anchors = await page.QuerySelectorAllAsync("a[href*='/video/']");
            var seen = new HashSet<string>(results.Select(r => r.Url));
            int before = results.Count;
            foreach (var a in anchors)
            {
                var href = await a.GetAttributeAsync("href");
                if (string.IsNullOrEmpty(href)) continue;
                if (!href.StartsWith("http")) href = "https://www.tiktok.com" + href;
                if (seen.Contains(href)) continue;

                var title = (await a.GetAttributeAsync("aria-label"))
                            ?? (await a.EvaluateAsync<string>("el => el.innerText"))
                            ?? string.Empty;
                // Try oEmbed for accurate caption/title
                var better = await TryFetchOEmbedTitleAsync(href, ct);
                if (!string.IsNullOrWhiteSpace(better)) title = better!;

                results.Add(new VideoDto { Title = title.Trim(), Url = href });
                if (results.Count >= max) break;
                seen.Add(href);
            }
            stagnant = (results.Count == before) ? stagnant + 1 : 0;
        }

        return results;
    }

    private static Task InstallBrowsersIfNeededAsync()
    {
        // Try to ensure browsers are installed; ignore failures.
        try { Microsoft.Playwright.Program.Main(new[] { "install", "chromium" }); } catch { }
        return Task.CompletedTask;
    }

    private async Task<string?> TryFetchOEmbedTitleAsync(string videoUrl, CancellationToken ct)
    {
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Get, $"https://www.tiktok.com/oembed?url={Uri.EscapeDataString(videoUrl)}");
            req.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0 Safari/537.36");
            using var resp = await _http.SendAsync(req, ct);
            if (!resp.IsSuccessStatusCode) return null;
            using var s = await resp.Content.ReadAsStreamAsync(ct);
            using var doc = await JsonDocument.ParseAsync(s, cancellationToken: ct);
            if (doc.RootElement.TryGetProperty("title", out var t) && t.ValueKind == JsonValueKind.String)
                return t.GetString();
        }
        catch { }
        return null;
    }

    private string? SelectCookieFor(string profileUrl, string? cookieKey)
    {
        try
        {
            var uri = new Uri(profileUrl);
            var host = uri.Host.ToLowerInvariant();
            var segs = uri.AbsolutePath.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
            var userSeg = segs.FirstOrDefault(s => s.StartsWith("@"));
            var username = userSeg?.TrimStart('@');

            if (!string.IsNullOrWhiteSpace(cookieKey))
            {
                var byName = _options.Cookies.FirstOrDefault(c => string.Equals(c.Name, cookieKey, StringComparison.OrdinalIgnoreCase));
                if (byName != null && !string.IsNullOrWhiteSpace(byName.Value)) return byName.Value;
            }

            var byProfile = _options.Cookies.FirstOrDefault(c => !string.IsNullOrWhiteSpace(c.Profile) && string.Equals(c.Profile!.TrimStart('@'), username, StringComparison.OrdinalIgnoreCase));
            if (byProfile != null && !string.IsNullOrWhiteSpace(byProfile.Value)) return byProfile.Value;

            var byHost = _options.Cookies.FirstOrDefault(c => !string.IsNullOrWhiteSpace(c.Host) && string.Equals(c.Host, host, StringComparison.OrdinalIgnoreCase));
            if (byHost != null && !string.IsNullOrWhiteSpace(byHost.Value)) return byHost.Value;

            var any = _options.Cookies.FirstOrDefault(c => string.IsNullOrWhiteSpace(c.Host) && string.IsNullOrWhiteSpace(c.Profile));
            if (any != null && !string.IsNullOrWhiteSpace(any.Value)) return any.Value;

            if (!string.IsNullOrWhiteSpace(_options.Cookie)) return _options.Cookie;
        }
        catch { }
        return null;
    }
}
