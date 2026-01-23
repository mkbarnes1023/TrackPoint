using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TrackPoint.Models;

namespace TrackPoint.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Domain DbSets
        public DbSet<CategoryModel> Categories { get; set; } = null!;
        public DbSet<LocationModel> Locations { get; set; } = null!;
        public DbSet<AssetModel> Assets { get; set; } = null!;
        public DbSet<AuditTrail> AuditTrail { get; set; } = null!;
        public DbSet<Category> Category { get; set; } = null!;
        public DbSet<Location> Location { get; set; } = null!;
        public DbSet<Asset> Asset { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<Asset> Asset { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Location> Location { get; set; }
    }
}