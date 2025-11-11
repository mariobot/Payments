using Hyip_Payments.Models;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Context
{
    public class PaymentsDbContext : DbContext
    {
        public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options)
            : base(options)
        {
        }

        public DbSet<CountryModel> Countries { get; set; }        
        public DbSet<MoneyModel> Money { get; set; }        
        public DbSet<UserModel> Users { get; set; }
        public DbSet<UserRoleModel> UserRoles { get; set; }
        public DbSet<RoleModel> Roles { get; set; }
        public DbSet<CoinModel> Coins { get; set; }
        public DbSet<PaymentModel> Payments { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<BrandModel> Brands { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<InvoiceModel> Invoices { get; set; }
        public DbSet<InvoiceItemModel> InvoiceItems { get; set; }
        public DbSet<WalletModel> Wallets { get; set; }
        public DbSet<PaymentMethodModel> PaymentMethods { get; set; }        
        public DbSet<PaymentTransactionModel> PaymentTransactions { get; set; }
        public DbSet<UserApplicationModel> UserApplications { get; set; }
        public DbSet<UserTenantModel> UserTenants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            System.Diagnostics.Debugger.Launch();
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
