using Microsoft.EntityFrameworkCore;

namespace TrackPoint.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Add DbSet<TEntity> properties here for your domain models.
    }
}