using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UPL.Domain.Entities;

namespace UPL.Data.Configurations;

public class IdCardConfiguration : IEntityTypeConfiguration<IdCard>
{
    public void Configure(EntityTypeBuilder<IdCard> builder)
    {
        builder.HasIndex(x => new { x.StudentId, x.CardNumber }).IsUnique();
        builder.HasOne(x => x.Student).WithOne(s => s.IdCard).HasForeignKey<IdCard>(x => x.StudentId).OnDelete(DeleteBehavior.Restrict);
    }
}

