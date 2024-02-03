using Calabonga.AspNetCore.AppDefinitions;
using Serilog;

try
{
	var builder = WebApplication.CreateBuilder(args);
	builder.Host.UseSerilog((context, configuration) =>
		configuration.ReadFrom.Configuration(context.Configuration));

	builder.AddDefinitions(typeof(Program));
	var app = builder.Build();
	app.UseDefinitions();
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
