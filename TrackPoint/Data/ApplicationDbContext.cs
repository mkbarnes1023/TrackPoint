using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TrackPoint.Models;

namespace TrackPoint.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { 
        }
        public DbSet<Asset> Asset { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Location> Location { get; set; }
    }
}