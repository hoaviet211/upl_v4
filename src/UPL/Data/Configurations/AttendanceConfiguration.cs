using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UPL.Domain.Entities;

namespace UPL.Data.Configurations;

public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.HasIndex(x => new { x.EnrollmentId, x.ClassSessionId }).IsUnique();
        builder.HasOne(x => x.Enrollment).WithMany(e => e.Attendances).HasForeignKey(x => x.EnrollmentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ClassSession).WithMany().HasForeignKey(x => x.ClassSessionId).OnDelete(DeleteBehavior.Restrict);
    }
}

