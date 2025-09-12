namespace UPL.Models;

public class YouTubePageResult
{
    public IReadOnlyList<VideoDto> Items { get; set; } = Array.Empty<VideoDto>();
    public string? NextPageToken { get; set; }
    public long? TotalCount { get; set; }
}

