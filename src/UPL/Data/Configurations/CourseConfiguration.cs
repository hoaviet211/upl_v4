using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UPL.Domain.Entities;

namespace UPL.Data.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasIndex(x => x.CourseCode).IsUnique();
        builder.HasIndex(x => new { x.ProgrammeId, x.Status, x.StartDate, x.EndDate });
        builder.HasOne(x => x.Programme).WithMany(p => p.Courses).HasForeignKey(x => x.ProgrammeId).OnDelete(DeleteBehavior.Restrict);
    }
}

