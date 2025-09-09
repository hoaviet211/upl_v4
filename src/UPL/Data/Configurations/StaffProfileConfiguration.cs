using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UPL.Domain.Entities;

namespace UPL.Data.Configurations;

public class StaffProfileConfiguration : IEntityTypeConfiguration<StaffProfile>
{
    public void Configure(EntityTypeBuilder<StaffProfile> builder)
    {
        builder.HasKey(x => x.UserId);
        builder.HasOne(x => x.User).WithOne(u => u.StaffProfile).HasForeignKey<StaffProfile>(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}

