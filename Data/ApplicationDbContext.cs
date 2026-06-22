using HOLA.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HOLA.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // 🚨 THIS LINE IS REQUIRED FOR THE FORUM TO WORK 🚨
        public DbSet<Post> Posts { get; set; }
    }
}