namespace UPL.Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Type { get; set; }

    public ICollection<Article> Articles { get; set; } = new List<Article>();
}

