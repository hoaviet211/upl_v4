using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UPL.Domain.Entities;

namespace UPL.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.HasIndex(x => x.EnrollmentId);
        builder.HasIndex(x => x.WorkshopRegistrationId);
        builder.HasIndex(x => new { x.Status, x.PaidAt });
        builder.HasIndex(x => x.TxnRef).IsUnique();
        builder.HasOne(x => x.Enrollment).WithMany(e => e.Payments).HasForeignKey(x => x.EnrollmentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.WorkshopRegistration).WithMany(r => r.Payments).HasForeignKey(x => x.WorkshopRegistrationId).OnDelete(DeleteBehavior.Restrict);
    }
}

