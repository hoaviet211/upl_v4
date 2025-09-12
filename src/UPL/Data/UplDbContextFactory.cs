using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace UPL.Data;

public class UplDbContextFactory : IDesignTimeDbContextFactory<UplDbContext>
{
    public UplDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UplDbContext>();

        // Prefer environment variable, then appsettings (respecting ASPNETCORE_ENVIRONMENT)
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        var basePath = Directory.GetCurrentDirectory();

        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__Default")
            ?? config.GetConnectionString("Default");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'Default' not found. Set env var ConnectionStrings__Default or add to appsettings.json.");
        }

        optionsBuilder.UseSqlServer(connectionString);
        return new UplDbContext(optionsBuilder.Options);
    }
}
