using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Hyip_Payments.Context
{
    public class PaymentsDbContextFactory : IDesignTimeDbContextFactory<PaymentsDbContext>
    {
        public PaymentsDbContext CreateDbContext(string[] args)
        {
            // Use a simplified connection string for design-time operations
            var connectionString = "Data Source=MBOTERO\\MBOTERO;Initial Catalog=Payments;Integrated Security=True;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;Encrypt=False";
            
            var optionsBuilder = new DbContextOptionsBuilder<PaymentsDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new PaymentsDbContext(optionsBuilder.Options);
        }
    }
}
