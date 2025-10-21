using Microsoft.EntityFrameworkCore;
using Hyip_Payments.Models;
using System.Diagnostics;

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
        public DbSet<CoinModel> Coins { get; set; }
        //public DbSet<PaymentMethodModel> PaymentMethods { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Debugger.Launch();
            base.OnModelCreating(modelBuilder);
            // Add custom configuration if needed
        }

        // Manual migration method(call this from your startup code)
        public void ApplyMigrations()
        {
            this.Database.Migrate();
        }
    }
}
