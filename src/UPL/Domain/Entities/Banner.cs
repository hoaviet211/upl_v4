namespace UPL.Domain.Entities;

public class Banner
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? ImageUrl { get; set; }
    public string? CtaText { get; set; }
    public string? CtaUrl { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}

