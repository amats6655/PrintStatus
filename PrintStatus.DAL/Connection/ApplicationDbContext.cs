using Microsoft.EntityFrameworkCore;
using PrintStatus.DOM.Models;

namespace PrintStatus.DAL.Connection
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<BasePrinterUser>()
				.HasKey(pu => new { pu.UserId, pu.BasePrinterId });


			builder.Entity<BasePrinterUser>()
				.HasOne(pu => pu.Printer)
				.WithMany(p => p.PrinterUsers)
				.HasForeignKey(pu => pu.BasePrinterId)
				.IsRequired()
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

		public DbSet<BasePrinter> BasePrinters { get; set; }
		public DbSet<History> Histories { get; set; }
		public DbSet<Location> Locations { get; set; }
		public DbSet<PrintModel> PrintModels { get; set; }
		public DbSet<PrintOid> Oids { get; set; }
		public DbSet<BasePrinterUser> BasePrinterUsers { get; set; }


	}
}
