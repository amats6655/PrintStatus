namespace PrintStatus.BLL.Services.Implementations;

using DOM.Models;
using Interfaces;
using Lextm.SharpSnmpLib;

public class PrinterDataCollectorService : IPrinterDataCollectorService
{
	private readonly ISnmpService _snmpService;
	private readonly IHistoryManagementService _historyManagement;
	private readonly IBasePrinterRepository _printerRepo;
	private readonly IPrintOidRepository _oidRepo;
	private readonly IPollingStateService _pollingStateService;

	public PrinterDataCollectorService(ISnmpService snmpService,
										IHistoryManagementService historyManagement,
										IBasePrinterRepository printerRepo,
										IPrintOidRepository oidRepo,
										IPollingStateService pollingStateService)
	{
		_historyManagement = historyManagement;
		_snmpService = snmpService;
		_printerRepo = printerRepo;
		_oidRepo = oidRepo;
		_pollingStateService = pollingStateService;
	}

	public async Task CollectAndScorePrinterDataAsync()
	{
		var printersResult = await _printerRepo.GetAllAsync();
		foreach (var printer in printersResult.Data)
		{
			var printerOids = await _oidRepo.GetAllByModelIdAsync(printer.PrintModelId);
			foreach (var oid in printerOids.Data)
			{
				// Проверяем, существует ли внешний ключ, и если нет, инициализируем внутренний словарь
				if (!_pollingStateService.LastPolledTimes.ContainsKey(printer.Id))
				{
					_pollingStateService.LastPolledTimes[printer.Id] = new Dictionary<int, DateTime>();
				}

				if (!_pollingStateService.LastPolledTimes[printer.Id].ContainsKey(oid.Id) || (DateTime.Now - _pollingStateService.LastPolledTimes[printer.Id][oid.Id]).TotalMinutes >= oid.PollingDate)
				{
					var result = await _snmpService.GetOidsAsync(printer.IpAddress, new List<Variable> { new Variable(new ObjectIdentifier(oid.Value)) });

					if (result.IsSuccess)
					{
						var history = new Journal
						{
							BasePrinterId = printer.Id,
							PrintOidId = oid.Id,
							Value = result.Data.First().Data.ToString(),
							Date = DateTime.Now
						};
						await _historyManagement.AddHistoryAsync(history);

						_pollingStateService.LastPolledTimes[printer.Id][oid.Id] = DateTime.Now;
					}
				}
			}
		}
	}
}

