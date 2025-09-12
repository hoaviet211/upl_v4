using UPL.Models;

namespace UPL.Infrastructure.Services.Video;

public interface IYouTubeVideoProvider
{
    // max: number of videos to retrieve; default int.MaxValue means all available
    Task<IReadOnlyList<VideoDto>> GetChannelVideosAsync(string profileUrl, int max = int.MaxValue, CancellationToken ct = default);

    // Page-wise retrieval for progress UIs
    Task<YouTubePageResult> GetChannelVideosPageAsync(string profileUrl, string? pageToken, int pageSize = 50, CancellationToken ct = default);

    // Estimated total count of videos on channel (statistics.videoCount)
    Task<long?> GetChannelVideoCountAsync(string profileUrl, CancellationToken ct = default);
}
