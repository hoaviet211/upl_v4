using Microsoft.AspNetCore.Mvc;
using UPL.Infrastructure.Services.Video;
using UPL.Models;

namespace UPL.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class VideosController : ControllerBase
    {
        private readonly IYouTubeVideoProvider _yt;
        private readonly ITikTokVideoProvider _tt;

    public VideosController(IYouTubeVideoProvider yt, ITikTokVideoProvider tt)
    {
        _yt = yt;
        _tt = tt;
    }

    private static bool IsAllowedProfileUrl(string url, params string[] allowedHosts)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return false;
        var host = uri.Host.ToLowerInvariant();
        return allowedHosts.Any(a => host == a || host.EndsWith("." + a));
    }

    // GET /api/videos/youtube?profileUrl=...&limit=...
    [HttpGet("youtube")]
    public async Task<ActionResult<IReadOnlyList<VideoDto>>> GetYouTube([FromQuery] string profileUrl, [FromQuery] int? limit, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(profileUrl)) return BadRequest("profileUrl is required");
        if (!IsAllowedProfileUrl(profileUrl, "youtube.com", "youtu.be")) return BadRequest("Invalid profileUrl host");
        try
        {
            var max = limit.HasValue && limit.Value > 0 ? limit.Value : int.MaxValue;
            var list = await _yt.GetChannelVideosAsync(profileUrl, max, ct);
            return Ok(list);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
        }
    }

    // GET /api/videos/tiktok?profileUrl=...
    [HttpGet("tiktok")]
    public async Task<ActionResult<IReadOnlyList<VideoDto>>> GetTikTok([FromQuery] string profileUrl, [FromQuery] bool? headless, [FromQuery] int? limit, [FromQuery] string? cookieKey, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(profileUrl)) return BadRequest("profileUrl is required");
        if (!IsAllowedProfileUrl(profileUrl, "tiktok.com", "m.tiktok.com")) return BadRequest("Invalid profileUrl host");
        var preferHeadless = headless.GetValueOrDefault(true);
        var max = limit.HasValue && limit.Value > 0 ? limit.Value : int.MaxValue;
        var list = await _tt.GetProfileVideosAsync(profileUrl, preferHeadless, max, cookieKey, ct);
        return Ok(list);
    }

    // Progress-friendly endpoints for YouTube
    // GET /api/videos/youtube/metadata?profileUrl=...
    [HttpGet("youtube/metadata")]
    public async Task<ActionResult<ChannelMetaDto>> GetYouTubeMeta([FromQuery] string profileUrl, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(profileUrl)) return BadRequest("profileUrl is required");
        if (!IsAllowedProfileUrl(profileUrl, "youtube.com", "youtu.be")) return BadRequest("Invalid profileUrl host");
        try
        {
            var total = await _yt.GetChannelVideoCountAsync(profileUrl, ct);
            return Ok(new ChannelMetaDto { TotalCount = total });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
        }
    }

    // GET /api/videos/youtube/page?profileUrl=...&pageToken=...&pageSize=50
    [HttpGet("youtube/page")]
    public async Task<ActionResult<YouTubePageResult>> GetYouTubePage([FromQuery] string profileUrl, [FromQuery] string? pageToken, [FromQuery] int pageSize = 50, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(profileUrl)) return BadRequest("profileUrl is required");
        if (!IsAllowedProfileUrl(profileUrl, "youtube.com", "youtu.be")) return BadRequest("Invalid profileUrl host");
        try
        {
            var page = await _yt.GetChannelVideosPageAsync(profileUrl, pageToken, pageSize, ct);
            return Ok(page);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
        }
    }
}
