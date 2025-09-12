using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using UPL.Models;

namespace UPL.Infrastructure.Services.Video;

public class YouTubeVideoProvider : IYouTubeVideoProvider
{
    private readonly HttpClient _http;
    private readonly YouTubeOptions _options;

    public YouTubeVideoProvider(HttpClient httpClient, IOptions<YouTubeOptions> options)
    {
        _http = httpClient;
        _options = options.Value;
    }

    public async Task<IReadOnlyList<VideoDto>> GetChannelVideosAsync(string profileUrl, int max = int.MaxValue, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
            throw new InvalidOperationException("YouTube API key is not configured.");

        var channelId = await ResolveChannelIdAsync(profileUrl, ct);
        if (string.IsNullOrEmpty(channelId))
            return Array.Empty<VideoDto>();

        var uploadsId = await GetUploadsPlaylistIdAsync(channelId, ct);
        if (string.IsNullOrEmpty(uploadsId))
            return Array.Empty<VideoDto>();

        var items = new List<VideoDto>(50);
        string? pageToken = null;
        while (true)
        {
            var take = Math.Min(50, Math.Max(1, max - items.Count));
            var url = $"https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&playlistId={uploadsId}&maxResults={take}&key={_options.ApiKey}" +
                      (pageToken != null ? $"&pageToken={Uri.EscapeDataString(pageToken)}" : string.Empty);
            using var resp = await _http.GetAsync(url, ct);
            resp.EnsureSuccessStatusCode();
            using var stream = await resp.Content.ReadAsStreamAsync(ct);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
            if (doc.RootElement.TryGetProperty("items", out var itemsEl))
            {
                foreach (var it in itemsEl.EnumerateArray())
                {
                    if (!it.TryGetProperty("snippet", out var sn)) continue;
                    if (!sn.TryGetProperty("title", out var titleEl)) continue;
                    if (!sn.TryGetProperty("resourceId", out var rid)) continue;
                    if (!rid.TryGetProperty("videoId", out var vidEl)) continue;
                    var title = titleEl.GetString() ?? string.Empty;
                    var videoId = vidEl.GetString() ?? string.Empty;
                    if (string.IsNullOrEmpty(videoId)) continue;
                    items.Add(new VideoDto
                    {
                        Title = title,
                        Url = $"https://www.youtube.com/watch?v={videoId}"
                    });
                }
            }

            pageToken = doc.RootElement.TryGetProperty("nextPageToken", out var nt) ? nt.GetString() : null;
            if (string.IsNullOrEmpty(pageToken) || items.Count >= max) break;
        }

        return items;
    }

    public async Task<YouTubePageResult> GetChannelVideosPageAsync(string profileUrl, string? pageToken, int pageSize = 50, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
            throw new InvalidOperationException("YouTube API key is not configured.");

        var channelId = await ResolveChannelIdAsync(profileUrl, ct);
        if (string.IsNullOrEmpty(channelId)) return new YouTubePageResult();

        var uploadsId = await GetUploadsPlaylistIdAsync(channelId, ct);
        if (string.IsNullOrEmpty(uploadsId)) return new YouTubePageResult();

        var take = Math.Clamp(pageSize, 1, 50);
        var url = $"https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&playlistId={uploadsId}&maxResults={take}&key={_options.ApiKey}" +
                  (string.IsNullOrEmpty(pageToken) ? string.Empty : $"&pageToken={Uri.EscapeDataString(pageToken)}");
        using var resp = await _http.GetAsync(url, ct);
        resp.EnsureSuccessStatusCode();
        using var stream = await resp.Content.ReadAsStreamAsync(ct);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

        var list = new List<VideoDto>(take);
        if (doc.RootElement.TryGetProperty("items", out var itemsEl))
        {
            foreach (var it in itemsEl.EnumerateArray())
            {
                if (!it.TryGetProperty("snippet", out var sn)) continue;
                if (!sn.TryGetProperty("title", out var titleEl)) continue;
                if (!sn.TryGetProperty("resourceId", out var rid)) continue;
                if (!rid.TryGetProperty("videoId", out var vidEl)) continue;
                var title = titleEl.GetString() ?? string.Empty;
                var videoId = vidEl.GetString() ?? string.Empty;
                if (string.IsNullOrEmpty(videoId)) continue;
                list.Add(new VideoDto { Title = title, Url = $"https://www.youtube.com/watch?v={videoId}" });
            }
        }

        var next = doc.RootElement.TryGetProperty("nextPageToken", out var nt) ? nt.GetString() : null;
        var total = await GetChannelVideoCountByIdAsync(channelId, ct);
        return new YouTubePageResult { Items = list, NextPageToken = next, TotalCount = total };
    }

    public async Task<long?> GetChannelVideoCountAsync(string profileUrl, CancellationToken ct = default)
    {
        var channelId = await ResolveChannelIdAsync(profileUrl, ct);
        if (string.IsNullOrEmpty(channelId)) return null;
        return await GetChannelVideoCountByIdAsync(channelId, ct);
    }

    private async Task<string?> ResolveChannelIdAsync(string profileUrl, CancellationToken ct)
    {
        var uri = new Uri(profileUrl);
        var path = uri.AbsolutePath.Trim('/');
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        // 1) /channel/UCxxxx[/...]
        if (segments.Length >= 2 && segments[0].Equals("channel", StringComparison.OrdinalIgnoreCase))
        {
            var cid = segments[1];
            return string.IsNullOrWhiteSpace(cid) ? null : cid;
        }

        // 2) /@handle[/videos|/streams|/shorts]
        var handleSeg = segments.FirstOrDefault(s => s.StartsWith("@"));
        if (!string.IsNullOrEmpty(handleSeg))
        {
            var handle = handleSeg.StartsWith("@") ? handleSeg : "@" + handleSeg.TrimStart('@');
            var url = $"https://www.googleapis.com/youtube/v3/channels?part=id&forHandle={Uri.EscapeDataString(handle)}&key={_options.ApiKey}";
            var id = await FirstChannelIdAsync(url, ct);
            if (!string.IsNullOrEmpty(id)) return id;
        }

        // 3) /user/<username>[/...]
        if (segments.Length >= 2 && segments[0].Equals("user", StringComparison.OrdinalIgnoreCase))
        {
            var username = segments[1];
            var url = $"https://www.googleapis.com/youtube/v3/channels?part=id&forUsername={Uri.EscapeDataString(username)}&key={_options.ApiKey}";
            var id = await FirstChannelIdAsync(url, ct);
            if (!string.IsNullOrEmpty(id)) return id;
        }

        // 4) /c/<custom>[/...] → no direct API; use search on the identifier part
        if (segments.Length >= 2 && segments[0].Equals("c", StringComparison.OrdinalIgnoreCase))
        {
            var custom = segments[1];
            var id = await SearchChannelIdAsync(custom, ct);
            if (!string.IsNullOrEmpty(id)) return id;
        }

        // 5) Fallback: try first non-generic segment (skip common tails like videos/shorts/streams)
        var skip = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "videos", "shorts", "streams", "featured" };
        var candidate = segments.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s) && !skip.Contains(s) && !s.Equals("@", StringComparison.Ordinal));
        if (!string.IsNullOrEmpty(candidate))
        {
            // Remove leading '@' if present for search
            candidate = candidate.TrimStart('@');
            var id = await SearchChannelIdAsync(candidate, ct);
            if (!string.IsNullOrEmpty(id)) return id;
        }

        return null;
    }

    private async Task<long?> GetChannelVideoCountByIdAsync(string channelId, CancellationToken ct)
    {
        var url = $"https://www.googleapis.com/youtube/v3/channels?part=statistics&id={Uri.EscapeDataString(channelId)}&key={_options.ApiKey}";
        using var resp = await _http.GetAsync(url, ct);
        resp.EnsureSuccessStatusCode();
        using var stream = await resp.Content.ReadAsStreamAsync(ct);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
        if (doc.RootElement.TryGetProperty("items", out var items))
        {
            var first = items.EnumerateArray().FirstOrDefault();
            if (first.ValueKind != JsonValueKind.Undefined && first.TryGetProperty("statistics", out var st))
            {
                if (st.TryGetProperty("videoCount", out var vc) && vc.ValueKind == JsonValueKind.String)
                {
                    if (long.TryParse(vc.GetString(), out var n)) return n;
                }
            }
        }
        return null;
    }

    private async Task<string?> SearchChannelIdAsync(string queryText, CancellationToken ct)
    {
        var query = Uri.EscapeDataString(queryText);
        var url = $"https://www.googleapis.com/youtube/v3/search?part=snippet&type=channel&q={query}&maxResults=1&key={_options.ApiKey}";
        using var resp = await _http.GetAsync(url, ct);
        resp.EnsureSuccessStatusCode();
        using var stream = await resp.Content.ReadAsStreamAsync(ct);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
        if (doc.RootElement.TryGetProperty("items", out var items))
        {
            var first = items.EnumerateArray().FirstOrDefault();
            if (first.ValueKind != JsonValueKind.Undefined && first.TryGetProperty("id", out var idObj) && idObj.TryGetProperty("channelId", out var chId))
            {
                return chId.GetString();
            }
        }
        return null;
    }

    private async Task<string?> FirstChannelIdAsync(string url, CancellationToken ct)
    {
        using var resp = await _http.GetAsync(url, ct);
        resp.EnsureSuccessStatusCode();
        using var stream = await resp.Content.ReadAsStreamAsync(ct);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
        if (doc.RootElement.TryGetProperty("items", out var items))
        {
            var first = items.EnumerateArray().FirstOrDefault();
            if (first.ValueKind != JsonValueKind.Undefined)
            {
                if (first.TryGetProperty("id", out var idEl))
                {
                    return idEl.GetString();
                }
            }
        }
        return null;
    }

    private async Task<string?> GetUploadsPlaylistIdAsync(string channelId, CancellationToken ct)
    {
        var url = $"https://www.googleapis.com/youtube/v3/channels?part=contentDetails&id={Uri.EscapeDataString(channelId)}&key={_options.ApiKey}";
        using var resp = await _http.GetAsync(url, ct);
        resp.EnsureSuccessStatusCode();
        using var stream = await resp.Content.ReadAsStreamAsync(ct);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
        if (doc.RootElement.TryGetProperty("items", out var items))
        {
            var first = items.EnumerateArray().FirstOrDefault();
            if (first.ValueKind != JsonValueKind.Undefined && first.TryGetProperty("contentDetails", out var cd))
            {
                if (cd.TryGetProperty("relatedPlaylists", out var rp) && rp.TryGetProperty("uploads", out var upl))
                {
                    return upl.GetString();
                }
            }
        }
        return null;
    }
}
