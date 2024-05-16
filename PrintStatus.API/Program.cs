using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PrintStatus.BLL.Helpers;
using PrintStatus.BLL.Services.Implementations;
using PrintStatus.BLL.Services.Interfaces;
using PrintStatus.DAL.Data;
using PrintStatus.DAL.Helpers;
using PrintStatus.DAL.Repositories.Implementations;
using PrintStatus.DAL.Repositories.Interfaces;
using PrintStatus.DOM.Models;
using Serilog;

try
{
	var builder = WebApplication.CreateBuilder(args);
	Log.Logger = new LoggerConfiguration()
		.Enrich.FromLogContext()
		.WriteTo.Console()
		.CreateLogger();
	builder.Host.ConfigureLogging(logging =>
	{
		logging.AddSerilog();
		logging.SetMinimumLevel(LogLevel.Information);
	}).UseSerilog();

	
	builder.Services.AddControllers();
	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGen(setup =>
	{
		// Include 'SecurityScheme' to use JWT Authentication
		var jwtSecurityScheme = new OpenApiSecurityScheme
		{
			BearerFormat = "JWT",
			Name = "JWT Authentication",
			In = ParameterLocation.Header,
			Type = SecuritySchemeType.Http,
			Scheme = JwtBearerDefaults.AuthenticationScheme,
			Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

			Reference = new OpenApiReference
			{
				Id = JwtBearerDefaults.AuthenticationScheme,
				Type = ReferenceType.SecurityScheme
			}
		};

		setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

		setup.AddSecurityRequirement(new OpenApiSecurityRequirement
		{
			{ jwtSecurityScheme, Array.Empty<string>() }
		});
		
		setup.SwaggerDoc("v1", new OpenApiInfo
		{
			Version = "v1",
			Title = "PrintStatus.API",
			Description = "Сервис для сбора статистики принтеров с помощью SNMP",
			Contact = new OpenApiContact
			{
				Name = "Telegram",
				Url = new Uri("https://t.me/amats")
			}
		});
		
		
		var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
		setup.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
	});

	string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

	builder.Services.AddDbContext<AppDbContext>(options =>
		options.UseNpgsql(connectionString));


	builder.Services.Configure<JwtSection>(builder.Configuration.GetSection("JwtSection"));
	var jwtSection = builder.Configuration.GetSection(nameof(JwtSection)).Get<JwtSection>();

	builder.Services.AddAuthentication(options =>
	{
		options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	}).AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateIssuerSigningKey = true,
			ValidateLifetime = true,
			ValidIssuer = jwtSection?.Issuer,
			ValidAudience = jwtSection?.Audience,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection?.Key!))
		};
	});

	builder.Services.AddScoped<IUserAccount, UserAccountRepository>();
	builder.Services.AddScoped<IPrinterRepository, PrinterRepository>();
	builder.Services.AddScoped<ILocationRepository, LocationRepository>();
	builder.Services.AddScoped<IPrintOidRepository, PrintOidRepository>();
	builder.Services.AddScoped<IPrintModelRepository, PrintModelRepository>();
	builder.Services.AddScoped<IConsumableRepository, ConsumableRepository>();
	builder.Services.AddScoped<IJournalRepository, JournalRepository>();
	builder.Services.AddScoped<ICalcTypeRepository, CalcTypeRepository>();
	builder.Services.AddScoped<IPrinterService, PrinterService>();
	builder.Services.AddScoped<IJournalService, JournalService>();
	builder.Services.AddScoped<ILocationService, LocationService>();
	builder.Services.AddScoped<IPrintOidService, PrintOidService>();
	builder.Services.AddScoped<IConsumableService, ConsumableService>();
	builder.Services.AddScoped<IPrinterDataCollectorService, PrinterDataCollectorService>();
	builder.Services.AddScoped<IPrintModelService, PrintModelService>();
	builder.Services.AddScoped<ICalcTypeService, CalcTypeService>();
	builder.Services.AddScoped<ISnmpService, SnmpService>();
	builder.Services.AddHostedService<PrinterDataCollectorHostedService>();
	builder.Services.AddSingleton<IPollingStateService, PollingStateService>();
	builder.Services.AddAutoMapper(typeof(AppMappingProfile));

	builder.Services.AddCors(options =>
	{
		options.AddPolicy("AllowBlazorWasm",
			builder => builder
				.WithOrigins("https://localhost:7095")
				.AllowAnyMethod()
				.AllowAnyHeader()
				.AllowCredentials());
	});
	var app = builder.Build();
	if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI();
	}

	app.UseHttpsRedirection();
	app.UseCors("AllowBlazorWasm");
	app.UseAuthentication();
	app.UseAuthorization();
	app.MapControllers();
	app.UseSerilogRequestLogging();
	app.Run();
	return 0;
}
catch (Exception ex)
{
	var type = ex.GetType().Name;
	if (type.Equals("HostAbortedException", StringComparison.Ordinal))
	{
		throw;
	}

	Log.Fatal(ex, "Unhandled exception");
	return 1;
}
finally
{
	Log.CloseAndFlush();
}