using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UPL.Domain.Entities;

namespace UPL.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.FullName).HasMaxLength(200);
        builder.Property(x => x.Email).HasMaxLength(256);
        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => new { x.IsActive, x.CreatedAt });

        builder
            .HasOne(u => u.Student)
            .WithOne(s => s.User)
            .HasForeignKey<Student>(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(u => u.StaffProfile)
            .WithOne(s => s.User)
            .HasForeignKey<StaffProfile>(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

