using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Hyip_Payments.Context
{
    /// <summary>
    /// Design-time factory for creating DbContext instances during migrations
    /// Reads connection string from appsettings.json instead of hardcoding
    /// </summary>
    public class PaymentsDbContextFactory : IDesignTimeDbContextFactory<PaymentsDbContext>
    {
        public PaymentsDbContext CreateDbContext(string[] args)
        {
            // Build configuration from appsettings files
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                // Try to read from API project first
                .AddJsonFile(Path.Combine("..", "Hyip_Payments.Api", "appsettings.json"), optional: true)
                .AddJsonFile(Path.Combine("..", "Hyip_Payments.Api", "appsettings.Development.json"), optional: true)
                // Fallback to Web project
                .AddJsonFile(Path.Combine("..", "Hyip-Payments.Web", "Hyip-Payments.Web", "appsettings.json"), optional: true)
                .AddJsonFile(Path.Combine("..", "Hyip-Payments.Web", "Hyip-Payments.Web", "appsettings.Development.json"), optional: true)
                // Add user secrets (for sensitive data like passwords)
                .AddUserSecrets<PaymentsDbContextFactory>(optional: true)
                // Add environment variables
                .AddEnvironmentVariables()
                .Build();

            // Get connection string from configuration
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' not found. " +
                    "Please ensure it's configured in appsettings.json or user secrets.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<PaymentsDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new PaymentsDbContext(optionsBuilder.Options);
        }
    }
}
