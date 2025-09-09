using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UPL.Domain.Entities;

namespace UPL.Data;

public class UplDbContext : DbContext
{
    public UplDbContext(DbContextOptions<UplDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<StaffProfile> StaffProfiles => Set<StaffProfile>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<IdCard> IdCards => Set<IdCard>();

    public DbSet<Programme> Programmes => Set<Programme>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseRequirement> CourseRequirements => Set<CourseRequirement>();
    public DbSet<ClassSession> ClassSessions => Set<ClassSession>();

    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<Payment> Payments => Set<Payment>();

    public DbSet<Workshop> Workshops => Set<Workshop>();
    public DbSet<WorkshopRegistration> WorkshopRegistrations => Set<WorkshopRegistration>();
    public DbSet<WorkshopAttendance> WorkshopAttendances => Set<WorkshopAttendance>();

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Article> Articles => Set<Article>();

    public DbSet<ApplicationConfig> ApplicationConfigs => Set<ApplicationConfig>();
    public DbSet<ContactInfo> ContactInfos => Set<ContactInfo>();
    public DbSet<Banner> Banners => Set<Banner>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UplDbContext).Assembly);

        // Seed minimal roles and users
        var hasher = new PasswordHasher<User>();

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Staff" },
            new Role { Id = 3, Name = "Student" }
        );

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                FullName = "System Admin",
                Email = "admin@gmail.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = hasher.HashPassword(null!, "testing")
            },
            new User
            {
                Id = 2,
                FullName = "Test Student",
                Email = "tam@gmail.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = hasher.HashPassword(null!, "testing")
            }
        );

        modelBuilder.Entity<UserRole>().HasData(
            new UserRole { UserId = 1, RoleId = 1 },
            new UserRole { UserId = 2, RoleId = 3 }
        );

        modelBuilder.Entity<Student>().HasData(
            new Student
            {
                Id = 1,
                UserId = 2,
                StudentCode = "ST001",
                Email = "tam@gmail.com",
                PhoneNumber = "0900000000"
            }
        );
    }
}

