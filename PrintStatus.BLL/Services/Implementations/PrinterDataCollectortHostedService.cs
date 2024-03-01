namespace PrintStatus.BLL.Services.Implementations;

using Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class PrinterDataCollectortHostedService : BackgroundService
{
	private readonly IServiceScopeFactory _serviceScopeFactory;
	public PrinterDataCollectortHostedService(IServiceScopeFactory serviceScopeFactory)
	{
		_serviceScopeFactory = serviceScopeFactory;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			using var scope = _serviceScopeFactory.CreateScope();
			var printerDataCollectorService = scope.ServiceProvider.GetRequiredService<IPrinterDataCollectorService>();
			await printerDataCollectorService.CollectAndScorePrinterDataAsync();
			await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
		}
	}
}

