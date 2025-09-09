using UPL.Domain.Enums;

namespace UPL.Domain.Entities;

public class Course
{
    public int Id { get; set; }
    public int ProgrammeId { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string? DescriptionShort { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Status Status { get; set; } = Status.Draft;

    public Programme Programme { get; set; } = null!;
    public ICollection<CourseRequirement> Requirements { get; set; } = new List<CourseRequirement>();
    public ICollection<ClassSession> ClassSessions { get; set; } = new List<ClassSession>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}

