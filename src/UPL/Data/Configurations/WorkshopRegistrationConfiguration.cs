using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UPL.Domain.Entities;

namespace UPL.Data.Configurations;

public class WorkshopRegistrationConfiguration : IEntityTypeConfiguration<WorkshopRegistration>
{
    public void Configure(EntityTypeBuilder<WorkshopRegistration> builder)
    {
        builder.HasIndex(x => new { x.WorkshopId, x.StudentId }).IsUnique();
        builder.HasOne(x => x.Workshop).WithMany(w => w.Registrations).HasForeignKey(x => x.WorkshopId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Student).WithMany(s => s.WorkshopRegistrations).HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Restrict);
    }
}

