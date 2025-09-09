using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UPL.Domain.Entities;

namespace UPL.Data.Configurations;

public class WorkshopAttendanceConfiguration : IEntityTypeConfiguration<WorkshopAttendance>
{
    public void Configure(EntityTypeBuilder<WorkshopAttendance> builder)
    {
        builder.HasOne(x => x.Registration).WithMany(r => r.WorkshopAttendances).HasForeignKey(x => x.RegistrationId).OnDelete(DeleteBehavior.Restrict);
    }
}

