using UPL.Models;

namespace UPL.Infrastructure.Services.Video;

public interface ITikTokVideoProvider
{
    Task<IReadOnlyList<VideoDto>> GetProfileVideosAsync(string profileUrl, int max = int.MaxValue, CancellationToken ct = default);

    Task<IReadOnlyList<VideoDto>> GetProfileVideosAsync(string profileUrl, bool preferHeadless, int max = int.MaxValue, string? cookieKey = null, CancellationToken ct = default);
}
