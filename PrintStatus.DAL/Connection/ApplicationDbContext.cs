using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PrintStatus.DOM.Models;

namespace PrintStatus.DAL.Connection
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserProfile>()
                .HasOne<IdentityUser>()
                .WithOne()
                .HasForeignKey<UserProfile>(up => up.IdentityId);
        }

        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<BasePrinter> BasePrinters { get; set; } = null!;
        public DbSet<History> Histories { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<PrintModel> PrintModels { get; set; }
        public DbSet<Oid> Oids { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }


    }
}
