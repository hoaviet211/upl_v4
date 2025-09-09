namespace UPL.Domain.Entities;

public class Article
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? Content { get; set; }
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int AuthorId { get; set; }

    public Category Category { get; set; } = null!;
    public User Author { get; set; } = null!;
}

