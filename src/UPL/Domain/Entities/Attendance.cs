namespace UPL.Domain.Entities;

public class Attendance
{
    public int Id { get; set; }
    public int EnrollmentId { get; set; }
    public int ClassSessionId { get; set; }
    public string? Status { get; set; }
    public string? Method { get; set; }
    public DateTime? CheckInTime { get; set; }
    public string? Note { get; set; }

    public Enrollment Enrollment { get; set; } = null!;
    public ClassSession ClassSession { get; set; } = null!;
}

