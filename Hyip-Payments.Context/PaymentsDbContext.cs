using Microsoft.EntityFrameworkCore;
using Hyip_Payments.Models;

namespace Hyip_Payments.Server.Data
{
    public class PaymentsDbContext : DbContext
    {
        public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options)
            : base(options)
        {
        }

        public DbSet<PaymentModel> Payments { get; set; }
        public DbSet<MoneyModel> Money { get; set; }
        //public DbSet<UserModel> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Add custom configuration if needed
        }
    }
}
