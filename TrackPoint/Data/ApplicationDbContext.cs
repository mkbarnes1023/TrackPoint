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
        public DbSet<AssetModel> Assets { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<LocationModel> Locations { get; set; }
    }
}