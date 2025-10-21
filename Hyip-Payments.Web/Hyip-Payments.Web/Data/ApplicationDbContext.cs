using System.Diagnostics;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Web.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Debugger.Launch();
            base.OnModelCreating(modelBuilder);

            // Add your custom configuration here
            // Example: modelBuilder.Entity<ApplicationUser>().Property(u => u.Email).IsRequired();

            // You can also seed data, configure relationships, etc.
        }

    }
}
