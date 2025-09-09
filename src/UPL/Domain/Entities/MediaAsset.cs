namespace UPL.Domain.Entities;

public class MediaAsset
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public string? Type { get; set; }
    public int? OwnerId { get; set; }
    public string? Source { get; set; }
}

