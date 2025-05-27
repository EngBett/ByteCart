using ByteCart.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ByteCart.Infrastructure.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Get the directory where the Infrastructure project is located
        var projectDir = Directory.GetCurrentDirectory();
        
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(projectDir)
            .AddJsonFile(Path.Combine(projectDir, "appsettings.json"), optional: true)
            .AddJsonFile(Path.Combine(projectDir, "appsettings.Development.json"), optional: true)
            // Also try Presentation directory
            .AddJsonFile(Path.Combine(projectDir, "..", "Presentation", "appsettings.json"), optional: true)
            .AddJsonFile(Path.Combine(projectDir, "..", "Presentation", "appsettings.Development.json"), optional: true)
            .AddEnvironmentVariables();

        var config = configBuilder.Build();
        var connectionString = config.GetConnectionString("ByteCartDb");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            // Fallback to hardcoded connection string if none found in config
            connectionString = "Server=127.0.0.1;Port=5432;Database=ByteCart;Username=super_admin;Password=password;";
        }

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
