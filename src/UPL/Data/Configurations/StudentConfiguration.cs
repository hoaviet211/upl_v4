using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UPL.Domain.Entities;

namespace UPL.Data.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasIndex(x => x.StudentCode).IsUnique();
        builder.HasOne(x => x.User).WithOne(u => u.Student).HasForeignKey<Student>(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}

