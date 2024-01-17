using System.Text;
using Calabonga.AspNetCore.AppDefinitions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PrintStatus.BLL;
using PrintStatus.BLL.Helpers;
using PrintStatus.BLL.Interfaces;
using PrintStatus.BLL.Services;
using PrintStatus.DAL.Connection;
using PrintStatus.DAL.Repositories;
using PrintStatus.DOM.Interfaces;

namespace PrintStatus.API.Definitions.Commons
{
	/// <summary>
	/// Common definition
	/// </summary>
	public class CommonDefinition : AppDefinition
	{
		public override void ConfigureServices(WebApplicationBuilder builder)
		{
			builder.Services.AddIdentity<IdentityUser, IdentityRole>()
								.AddEntityFrameworkStores<ApplicationDbContext>()
								.AddDefaultTokenProviders();

			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			})
				.AddJwtBearer(options =>
				{
					options.SaveToken = true;
					options.RequireHttpsMetadata = false;
					options.TokenValidationParameters = new TokenValidationParameters()
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidAudience = builder.Configuration["JWT: ValidAudience"],
						ValidIssuer = builder.Configuration["JWT: ValidIssuer"],
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT: SecretKey"]))
					};
				});
			builder.Services.AddAuthorization();
			// Add services to the container.
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(swagger =>
			{
				swagger.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = "v1",
					Title = "PrintStatus",
					Description = "Сервис обработки данных от принтеров"
				});
				swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
				{
					Name = "Authorization",
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer",
					BearerFormat = "JWT",
					In = ParameterLocation.Header,
					Description = "Enter ‘Bearer’ [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
				});
				swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
					new OpenApiSecurityScheme
						{
						Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							}
						},
					new string[] {}

					}
				});
			});
			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
			Console.WriteLine(connectionString);
			builder.Services.ConfigureDALServices(connectionString);

			builder.Services.AddScoped<IBasePrinterManagementService, BasePrinterManagementService>();
			builder.Services.AddScoped<IBasePrinterRepository, BasePrinterRepository>();
			builder.Services.AddScoped<IHistoryManagementService, HistoryManagementService>();
			builder.Services.AddScoped<IHistoryRepository, HistoryRepository>();
			builder.Services.AddScoped<ILocationManagementService, LocationManagementService>();
			builder.Services.AddScoped<ILocationRepository, LocationRepository>();
			builder.Services.AddScoped<IPrintOidManagementService, OidManagementService>();
			builder.Services.AddScoped<IPrintOidRepository, PrintOidRepository>();
			builder.Services.AddScoped<IPrinterDataCollectorService, PrinterDataCollectorService>();
			builder.Services.AddScoped<IPrintModelManagementService, PrintModelManagementService>();
			builder.Services.AddScoped<IPrintModelRepository, PrintModelRepository>();
			builder.Services.AddScoped<ISnmpService, SnmpService>();
			builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
			builder.Services.AddScoped<IAccountService, AccountService>();
			builder.Services.AddScoped<IUserService, UserService>();
			builder.Services.AddAutoMapper(typeof(AppMappingProfile));
		}

		public override void ConfigureApplication(WebApplication app)
		{
			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}
			app.UseAuthentication();
			app.UseAuthorization();
			app.UseStaticFiles();

			app.UseHttpsRedirection();
			app.MapControllers();
		}
	}
}