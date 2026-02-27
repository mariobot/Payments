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
        public DbSet<CustomerModel> Customers { get; set; }
        public DbSet<InvoiceModel> Invoices { get; set; }
        public DbSet<InvoiceItemModel> InvoiceItems { get; set; }
        public DbSet<WalletModel> Wallets { get; set; }
        public DbSet<PaymentMethodModel> PaymentMethods { get; set; }        
        public DbSet<PaymentTransactionModel> PaymentTransactions { get; set; }
        public DbSet<UserApplicationModel> UserApplications { get; set; }
        public DbSet<UserTenantModel> UserTenants { get; set; }
        public DbSet<CustomReportModel> CustomReports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //System.Diagnostics.Debugger.Launch();
            base.OnModelCreating(modelBuilder);
            
            // Configure decimal precision for WalletModel.Balance
            modelBuilder.Entity<WalletModel>()
                .Property(w => w.Balance)
                .HasPrecision(18, 2);

            // Configure JSON column for CustomReportModel
            modelBuilder.Entity<CustomReportModel>()
                .Property(r => r.ConfigurationJson)
                .HasColumnType("nvarchar(max)");

            // Configure Customer entity
            modelBuilder.Entity<CustomerModel>(entity =>
            {
                entity.HasKey(e => e.Id);

                // CustomerNumber must be unique
                entity.HasIndex(e => e.CustomerNumber)
                    .IsUnique();

                entity.Property(e => e.CustomerNumber)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ContactName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20);

                entity.Property(e => e.Address)
                    .HasMaxLength(500);

                entity.Property(e => e.City)
                    .HasMaxLength(100);

                entity.Property(e => e.State)
                    .HasMaxLength(100);

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(20);

                entity.Property(e => e.TaxId)
                    .HasMaxLength(50);

                entity.Property(e => e.CurrentBalance)
                    .HasPrecision(18, 2);

                entity.Property(e => e.CreditLimit)
                    .HasPrecision(18, 2);

                entity.Property(e => e.DiscountPercentage)
                    .HasPrecision(5, 4); // Allows up to 99.9999%

                entity.Property(e => e.Notes)
                    .HasMaxLength(1000);

                // Relationship with Country
                entity.HasOne(e => e.Country)
                    .WithMany()
                    .HasForeignKey(e => e.CountryId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Relationship with Invoices
                entity.HasMany(e => e.Invoices)
                    .WithOne(e => e.Customer)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            //this.Database.Migrate(); // Ensure migrations are applied at startup - use with caution in production
        }

        // Manual migration method(call this from your startup code)
        public void ApplyMigrations()
        {
            this.Database.Migrate();
        }
    }
}
