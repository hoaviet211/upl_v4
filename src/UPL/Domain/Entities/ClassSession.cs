using UPL.Domain.Enums;

namespace UPL.Domain.Entities;

public class ClassSession
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string SessionCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? DurationMinutes { get; set; }
    public SessionMode Mode { get; set; } = SessionMode.Offline;
    public string? MeetingUrl { get; set; }
    public Status Status { get; set; } = Status.Active;

    public Course Course { get; set; } = null!;
}

