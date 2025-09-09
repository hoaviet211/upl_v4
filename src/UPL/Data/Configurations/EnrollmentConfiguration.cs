using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UPL.Domain.Entities;

namespace UPL.Data.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.Property(x => x.TuitionFee).HasPrecision(18, 2);
        builder.Property(x => x.Discount).HasPrecision(18, 2);
        builder.HasIndex(x => new { x.Status, x.RegisteredAt, x.PaymentStatus });
        builder.HasIndex(x => new { x.CourseId, x.StudentId }).IsUnique();
        builder.HasOne(x => x.Course).WithMany(c => c.Enrollments).HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Student).WithMany(s => s.Enrollments).HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Restrict);
    }
}

