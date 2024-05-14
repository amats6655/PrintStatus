namespace PrintStatus.DAL.Data;

using DOM.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
	public DbSet<ApplicationUser> ApplicationUsers { get; set; }
	public DbSet<CalcType> CalcTypes { get; set; }
	public DbSet<Journal> Journals { get; set; }
	public DbSet<Printer> Printers { get; set; }
	public DbSet<PrintModel> PrintModels { get; set; }
	public DbSet<PrintOid> PrintOids { get; set; }
	public DbSet<UserRole> UserRoles {get; set;}
	public DbSet<SystemRole> SystemRoles {get; set;}
	public DbSet<RefreshTokenInfo> RefreshTokenInfos {get; set;}
	public DbSet<Location> Locations { get; set; }
	public DbSet<Consumable> Consumables { get; set; }
}

