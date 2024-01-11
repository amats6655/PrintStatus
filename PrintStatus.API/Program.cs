using Microsoft.AspNetCore.Identity;
using PrintStatus.BLL;
using PrintStatus.BLL.Helpers;
using PrintStatus.BLL.Interfaces;
using PrintStatus.BLL.Services;
using PrintStatus.DAL.Connection;
using PrintStatus.DAL.Repositories;
using PrintStatus.DOM.Interfaces;
using PrintStatus.DOM.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
								.AddEntityFrameworkStores<ApplicationDbContext>()
								.AddDefaultTokenProviders();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.ConfigureDALServices(connectionString);

builder.Services.AddScoped<IBasePrinterManagementService, BasePrinterManagementService>();
builder.Services.AddScoped<IBasePrinterRepository, BasePrinterRepository>();
builder.Services.AddScoped<IAuditLogManagementService, AuditLogManagementService>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IHistoryManagementService, HistoryManagementService>();
builder.Services.AddScoped<IHistoryRepository, HistoryRepository>();
builder.Services.AddScoped<ILocationManagementService, LocationManagementService>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IOidManagementService, OidManagementService>();
builder.Services.AddScoped<IOidRepository, OidRepository>();
builder.Services.AddScoped<IPrinterDataCollectorService, PrinterDataCollectorService>();
builder.Services.AddScoped<IPrintModelManagementService, PrintModelManagementService>();
builder.Services.AddScoped<IPrintModelRepository, PrintModelRepository>();
builder.Services.AddScoped<ISnmpService, SnmpService>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IUserService, UserService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	// builder.Services.ConfigureDALServices(connectionString);
	app.UseSwagger();
	app.UseSwaggerUI();
	
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();