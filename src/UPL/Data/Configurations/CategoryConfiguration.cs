using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UPL.Domain.Entities;

namespace UPL.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasIndex(x => x.Slug).IsUnique();
        builder.Property(x => x.Name).UseCollation("Vietnamese_100_CI_AI");
        builder.Property(x => x.Slug).UseCollation("Vietnamese_100_CI_AI");
    }
}

