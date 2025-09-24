using Hyip_Payments.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Web.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        // is deffined in Hyip-Payments.Context project
        //public DbSet<CountryModel> Countries { get; set; }
    }
}
