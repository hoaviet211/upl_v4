using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UPL.Domain.Entities;

namespace UPL.Data.Configurations;

public class ProgrammeConfiguration : IEntityTypeConfiguration<Programme>
{
    public void Configure(EntityTypeBuilder<Programme> builder)
    {
        // no special indexes
    }
}

