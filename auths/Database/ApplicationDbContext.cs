using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using auths.Models;
using Microsoft.AspNetCore.Identity;

namespace auths.Database
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Custom DbSet for your application-specific entities
        


        public DbSet<Product> Products { get; set; }
       
    }
}
