namespace UPL.Domain.Entities;

public class WorkshopAttendance
{
    public int Id { get; set; }
    public int RegistrationId { get; set; }
    public string? Status { get; set; }
    public string? Method { get; set; }
    public DateTime? CheckInTime { get; set; }
    public string? Note { get; set; }

    public WorkshopRegistration Registration { get; set; } = null!;
}

