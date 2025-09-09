namespace UPL.Domain.Entities;

public class IdCard
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string CardNumber { get; set; } = string.Empty;
    public DateTime? DateOfIssue { get; set; }
    public string? PlaceOfIssue { get; set; }

    public Student Student { get; set; } = null!;
}

