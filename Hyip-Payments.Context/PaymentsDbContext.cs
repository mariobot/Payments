using Microsoft.EntityFrameworkCore;
using Hyip_Payments.Models;

namespace Hyip_Payments.Context
{
    public class PaymentsDbContext : DbContext
    {
        public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options)
            : base(options)
        {
        }

        public DbSet<CountryModel> Countries { get; set; }
        public DbSet<PaymentModel> Payments { get; set; }
        public DbSet<MoneyModel> Money { get; set; }        
        public DbSet<UserModel> Users { get; set; }
        public DbSet<InvoiceModel> Invoices { get; set; }
        public DbSet<InvoiceItemModel> InvoiceItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Add custom configuration if needed
            ApplyMigrations();
        }

        // Manual migration method(call this from your startup code)
        public void ApplyMigrations()
        {
            this.Database.Migrate();
        }
    }
}
