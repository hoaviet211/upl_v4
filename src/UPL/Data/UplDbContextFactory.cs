using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace UPL.Data;

public class UplDbContextFactory : IDesignTimeDbContextFactory<UplDbContext>
{
    public UplDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UplDbContext>();
        optionsBuilder.UseSqlServer("Server=.;Database=UPL;Trusted_Connection=True;TrustServerCertificate=True");
        return new UplDbContext(optionsBuilder.Options);
    }
}

