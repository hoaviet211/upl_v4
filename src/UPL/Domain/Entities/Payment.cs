using UPL.Domain.Enums;

namespace UPL.Domain.Entities;

public class Payment
{
    public int Id { get; set; }
    public int? EnrollmentId { get; set; }
    public int? WorkshopRegistrationId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "VND";
    public string? Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public DateTime? PaidAt { get; set; }
    public string TxnRef { get; set; } = string.Empty;

    public Enrollment? Enrollment { get; set; }
    public WorkshopRegistration? WorkshopRegistration { get; set; }
}

