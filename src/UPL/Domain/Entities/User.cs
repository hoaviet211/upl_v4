using System.ComponentModel.DataAnnotations;

namespace UPL.Domain.Entities;

public class User
{
    public int Id { get; set; }
    [MaxLength(200)] public string FullName { get; set; } = string.Empty;
    [MaxLength(256)] public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public Student? Student { get; set; }
    public StaffProfile? StaffProfile { get; set; }
    public ICollection<Article> Articles { get; set; } = new List<Article>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}

