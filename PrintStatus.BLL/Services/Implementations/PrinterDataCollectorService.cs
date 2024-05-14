using PrintStatus.DAL.Repositories.Implementations;
using PrintStatus.DAL.Repositories.Interfaces;

namespace PrintStatus.BLL.Services.Implementations;

using DOM.Models;
using Interfaces;
using Lextm.SharpSnmpLib;

public class PrinterDataCollectorService : IPrinterDataCollectorService
{
	private readonly ISnmpService _snmpService;
	private readonly IJournalService _journalService;
	private readonly IPrinterRepository _printerRepo;
	private readonly IPrintOidRepository _oidRepo;
	private readonly IPollingStateService _pollingStateService;
	private readonly IConsumableRepository _consumableRepo;

	public PrinterDataCollectorService(ISnmpService snmpService,
										IJournalService journalService,
										IPrinterRepository printerRepo,
										IPrintOidRepository oidRepo,
										IPollingStateService pollingStateService,
										IConsumableRepository consumableRepository)
	{
		_journalService = journalService;
		_snmpService = snmpService;
		_printerRepo = printerRepo;
		_oidRepo = oidRepo;
		_consumableRepo = consumableRepository;
		_pollingStateService = pollingStateService;
	}

	public async Task CollectAndScorePrinterDataAsync()
	{
		var printersResult = await _printerRepo.GetAllAsync();
		foreach (var printer in printersResult.Data)
		{
			var printerConsumables = await _consumableRepo.GetAllByModelIdAsync(printer.PrintModelId);
			foreach (var consumable in printerConsumables.Data)
			{
				// Проверяем, существует ли внешний ключ, и если нет, инициализируем внутренний словарь
				if (!_pollingStateService.LastPolledTimes.ContainsKey(printer.Id))
				{
					_pollingStateService.LastPolledTimes[printer.Id] = new Dictionary<int, DateTime>();
				}

				if (!_pollingStateService.LastPolledTimes[printer.Id].ContainsKey(consumable.Id) || 
				    (DateTime.Now - _pollingStateService.LastPolledTimes[printer.Id][consumable.Id]).TotalMinutes >= consumable.PrintOid!.PollingRate)
				{
					var result = await _snmpService.GetOidsAsync(printer.IpAddress, 
						new List<Variable> { new Variable(new ObjectIdentifier(consumable.PrintOid!.Value!)) });

					if (result.IsSuccess)
					{
						var history = new Journal
						{
							PrinterId = printer.Id,
							ConsumableId = consumable.Id,
							Value = result.Data.First().Data.ToString(),
							Date = DateTime.Now
						};
						await _journalService.InsertAsync(history);

						_pollingStateService.LastPolledTimes[printer.Id][consumable.Id] = DateTime.Now;
					}
				}
			}
		}
	}
}

