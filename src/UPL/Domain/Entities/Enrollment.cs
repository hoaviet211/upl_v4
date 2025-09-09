using UPL.Domain.Enums;

namespace UPL.Domain.Entities;

public class Enrollment
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public int StudentId { get; set; }
    public Status Status { get; set; } = Status.Active;
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public decimal TuitionFee { get; set; }
    public decimal Discount { get; set; }
    public string? Note { get; set; }

    public Course Course { get; set; } = null!;
    public Student Student { get; set; } = null!;
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

