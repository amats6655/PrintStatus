using Calabonga.AspNetCore.AppDefinitions;
using Microsoft.OpenApi.Models;
using PrintStatus.BLL.Helpers;
using PrintStatus.BLL.Interfaces;
using PrintStatus.BLL.Services;

namespace Api.Definitions.Commons
{
	using Microsoft.EntityFrameworkCore.Migrations;
	using PrintStatus.BLL.Services.Implementations;
	using PrintStatus.DAL.Repositories.Implementations;
	using PrintStatus.DAL.Repositories.Interfaces;
	using PrintStatus.DOM.Models;

	public class CommonDefinition : AppDefinition
	{
		public override void ConfigureServices(WebApplicationBuilder builder)
		{
			builder.Services.AddAuthentication("Bearer")
							//.AddJwtBearer("Bearer", options =>
							//{
							//	options.Authority = "https://localhost:5001";

							//	options.TokenValidationParameters = new TokenValidationParameters
							//	{
							//		ValidateAudience = false
							//	};
							//});
							.AddJwtBearer("Bearer", options =>
							{
								options.Authority = "https://localhost:5001";
								options.TokenValidationParameters.ValidateAudience = false;
								options.MapInboundClaims = false;

								options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
							});

			builder.Services.AddControllers();
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
			
			
			
			builder.Services.AddScoped<IBasePrinterManagementService, BasePrinterManagementService>();
			builder.Services.AddScoped<IGenericRepositoryInterface<Printer>, PrinterRepository>();
			builder.Services.AddScoped<IHistoryManagementService, HistoryManagementService>();
			builder.Services.AddScoped<IHistoryRepository, HistoryRepository>();
			builder.Services.AddScoped<ILocationManagementService, LocationManagementService>();
			builder.Services.AddScoped<IGenericRepositoryInterface<Location>, LocationRepository>();
			builder.Services.AddScoped<IPrintOidManagementService, OidManagementService>();
			builder.Services.AddScoped<IGenericRepositoryInterface<PrintOid>, PrintOidRepository>();
			builder.Services.AddScoped<IPrinterDataCollectorService, PrinterDataCollectorService>();
			builder.Services.AddScoped<IPrintModelManagementService, PrintModelManagementService>();
			builder.Services.AddScoped<IGenericRepositoryInterface<PrintModel>, PrintModelRepository>();
			builder.Services.AddScoped<ISnmpService, SnmpService>();
			builder.Services.AddHostedService<PrinterDataCollectortHostedService>();
			builder.Services.AddSingleton<IPollingStateService, PollingStateService>();
			builder.Services.AddAutoMapper(typeof(AppMappingProfile));


		}


		public override void ConfigureApplication(WebApplication app)
		{
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}
			app.UseAuthentication();
			app.UseAuthorization();

			app.UseHttpsRedirection();
			app.MapControllers().RequireAuthorization();
		}
	}
}
