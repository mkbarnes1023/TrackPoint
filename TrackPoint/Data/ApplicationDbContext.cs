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
        


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<Asset> Asset { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Location> Location { get; set; }
        public DbSet<ApprovalReason> ApprovalReason { get; set; }
        public DbSet<Approvals> Approvals { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<UserPreferences> UserPreferences { get; set; }
        public DbSet<AssetLoan> Assetloan { get; set; }
        public DbSet<AuditTrail> AuditTrail { get; set; }
        public DbSet<TransferLog> TransferLog { get; set; }

    } 
}