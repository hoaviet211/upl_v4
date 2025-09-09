namespace UPL.Domain.Entities;

public class StaffProfile
{
    public int UserId { get; set; }
    public string? Phone { get; set; }
    public string? ZaloNumber { get; set; }
    public string? Position { get; set; }
    public string? Note { get; set; }

    public User User { get; set; } = null!;
}

