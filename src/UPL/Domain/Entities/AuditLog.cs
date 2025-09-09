namespace UPL.Domain.Entities;

public class AuditLog
{
    public int Id { get; set; }
    public int ActorUserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string? BeforeJson { get; set; }
    public string? AfterJson { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User Actor { get; set; } = null!;
}

