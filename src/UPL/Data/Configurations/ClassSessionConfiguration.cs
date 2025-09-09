using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UPL.Domain.Entities;

namespace UPL.Data.Configurations;

public class ClassSessionConfiguration : IEntityTypeConfiguration<ClassSession>
{
    public void Configure(EntityTypeBuilder<ClassSession> builder)
    {
        builder.HasIndex(x => new { x.CourseId, x.SessionCode, x.StartTime });
        builder.HasOne(x => x.Course).WithMany(c => c.ClassSessions).HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.Restrict);
    }
}

