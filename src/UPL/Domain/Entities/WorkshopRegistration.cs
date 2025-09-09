namespace UPL.Domain.Entities;

public class WorkshopRegistration
{
    public int Id { get; set; }
    public int WorkshopId { get; set; }
    public int StudentId { get; set; }
    public int Status { get; set; }
    public DateTime RegisterDate { get; set; } = DateTime.UtcNow;

    public Workshop Workshop { get; set; } = null!;
    public Student Student { get; set; } = null!;
    public ICollection<WorkshopAttendance> WorkshopAttendances { get; set; } = new List<WorkshopAttendance>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

