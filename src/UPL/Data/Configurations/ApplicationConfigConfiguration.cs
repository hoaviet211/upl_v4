using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UPL.Domain.Entities;

namespace UPL.Data.Configurations;

public class ApplicationConfigConfiguration : IEntityTypeConfiguration<ApplicationConfig>
{
    public void Configure(EntityTypeBuilder<ApplicationConfig> builder)
    {
        builder.HasIndex(x => x.CodeConfig).IsUnique();
    }
}

