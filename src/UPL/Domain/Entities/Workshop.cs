using UPL.Domain.Enums;

namespace UPL.Domain.Entities;

public class Workshop
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Lecturer { get; set; }
    public string? Location { get; set; }
    public string? ImageUrl { get; set; }
    public Status Status { get; set; } = Status.Active;

    public ICollection<WorkshopRegistration> Registrations { get; set; } = new List<WorkshopRegistration>();
}

