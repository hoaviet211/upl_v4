namespace UPL.Domain.Enums;

public enum Status
{
    Inactive = 0,
    Active = 1,
    Draft = 2,
    Archived = 3
}

public enum PaymentStatus
{
    Pending = 0,
    Paid = 1,
    Failed = 2,
    Refunded = 3
}

public enum SessionMode
{
    Offline = 0,
    Online = 1,
    Hybrid = 2
}

