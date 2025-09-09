using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UPL.Domain.Entities;

namespace UPL.Data.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasIndex(x => new { x.UserId, x.IsRead });
        builder.HasIndex(x => x.CreatedAt);
        builder.HasOne(x => x.User).WithMany(u => u.Notifications).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}

