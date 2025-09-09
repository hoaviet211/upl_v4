using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UPL.Domain.Entities;

namespace UPL.Data.Configurations;

public class CourseRequirementConfiguration : IEntityTypeConfiguration<CourseRequirement>
{
    public void Configure(EntityTypeBuilder<CourseRequirement> builder)
    {
        builder.HasOne(x => x.Course).WithMany(c => c.Requirements).HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.Restrict);
    }
}

