namespace UPL.Domain.Entities;

public class CourseRequirement
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string Content { get; set; } = string.Empty;

    public Course Course { get; set; } = null!;
}

