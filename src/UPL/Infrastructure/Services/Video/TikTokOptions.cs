namespace UPL.Infrastructure.Services.Video;

public class TikTokCookie
{
    public string? Name { get; set; } // optional identifier
    public string? Host { get; set; } // e.g., www.tiktok.com or m.tiktok.com
    public string? Profile { get; set; } // e.g., @vienupl (with or without @)
    public string Value { get; set; } = string.Empty; // raw Cookie header value
}

public class TikTokOptions
{
    public const string SectionName = "TikTok";
    // Legacy single cookie
    public string? Cookie { get; set; }
    // Multiple cookies mapped by host/profile/name
    public List<TikTokCookie> Cookies { get; set; } = new();
}
