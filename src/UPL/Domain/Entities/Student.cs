namespace UPL.Domain.Entities;

public class Student
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? Birthday { get; set; }
    public string? ImagePath { get; set; }
    public string? Email { get; set; }
    public string? ZaloNumber { get; set; }

    public User User { get; set; } = null!;
    public IdCard? IdCard { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<WorkshopRegistration> WorkshopRegistrations { get; set; } = new List<WorkshopRegistration>();
}

