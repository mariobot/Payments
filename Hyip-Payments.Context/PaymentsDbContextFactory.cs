using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Hyip_Payments.Context
{
    public class PaymentsDbContextFactory : IDesignTimeDbContextFactory<PaymentsDbContext>
    {
        public PaymentsDbContext CreateDbContext(string[] args)
        {
            // Build configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Hyip-Payments.Web/Hyip-Payments.Web"))
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<PaymentsDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new PaymentsDbContext(optionsBuilder.Options);
        }
    }
}