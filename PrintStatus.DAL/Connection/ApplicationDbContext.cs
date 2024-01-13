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
				.HasForeignKey<UserProfile>(up => up.IdentityId)
				.OnDelete(DeleteBehavior.Cascade);
			builder.Entity<PrintModel>()
				.HasMany(p => p.Printers)
				.WithOne(p => p.PrintModel)
				.OnDelete(DeleteBehavior.Restrict);
			builder.Entity<Location>()
				.HasMany(p => p.Printers)
				.WithOne(p => p.Location)
				.OnDelete(DeleteBehavior.Restrict);
			builder.Entity<BasePrinter>()
				.HasMany(p => p.Histories)
				.WithOne(p => p.Printer)
				.OnDelete(DeleteBehavior.Cascade);
			builder.Entity<PrintOid>()
				.HasMany(p => p.Histories)
				.WithOne(p => p.PrintOid)
				.OnDelete(DeleteBehavior.Cascade);
			
		}

		public DbSet<BasePrinter> BasePrinters { get; set; } = null!;
		public DbSet<History> Histories { get; set; }
		public DbSet<Location> Locations { get; set; }
		public DbSet<PrintModel> PrintModels { get; set; }
		public DbSet<PrintOid> Oids { get; set; }
		public DbSet<UserProfile> UserProfiles { get; set; }


	}
}
