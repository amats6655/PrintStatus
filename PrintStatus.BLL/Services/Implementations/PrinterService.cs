namespace PrintStatus.BLL.Services.Implementations;

using PrintStatus.DAL.Repositories.Implementations;
using PrintStatus.DAL.Repositories.Interfaces;
using AutoMapper;
using DOM.Models;
using DTO;
using Helpers;
using Interfaces;
using Lextm.SharpSnmpLib;

public class PrinterService : IPrinterService
{
	private readonly IPrinterRepository _printRepo;
	private readonly IPrintModelService _modelService;
	private readonly ILocationService _locationService;
	private readonly IPrintOidService _oidService;
	private readonly ISnmpService _snmpService;
	private readonly IMapper _mapper;
	private readonly IUserAccount _userAccount;
	private readonly IConsumableService _consumableService;
	public PrinterService(
		IPrinterRepository printRepo,
		IPrintModelService modelService,
		ILocationService locationService,
		IPrintOidService oidService,
		ISnmpService snmpService,
		IMapper mapper,
		IUserAccount userAccount,
		IConsumableService consumableService)
	{
		_printRepo = printRepo;
		_modelService = modelService;
		_locationService = locationService;
		_oidService = oidService;
		_snmpService = snmpService;
		_mapper = mapper;
		_userAccount = userAccount;
		_consumableService = consumableService;
	}

	public async Task<IServiceResponse<bool>> InsertAsync(NewPrinterDTO printer, int userId)
	{
		var currentUser = await _userAccount.GetUserById(userId);
		var snmpResult = await _snmpService.GetModelAndSerialNumAsync(printer.IpAddress);
		if (!snmpResult.IsSuccess) return ServiceResponse<bool>.Failure(snmpResult.Message);
		
		var printerExist = await _printRepo.GetBySerialNumberAsync(snmpResult.Data["SerialNumber"]);
		if (printerExist.Errors.Any()) return ServiceResponse<bool>.Failure(printerExist.Message);

		if (!printerExist.IsSuccess)
		{
			var printModel = await _modelService.InsertAsync(snmpResult.Data["Model"]);
			if (!printModel.IsSuccess) return ServiceResponse<bool>.Failure(printModel.Message);

			var newPrinter = new Printer()
			{
				IpAddress = printer.IpAddress,
				Name = printer.Name,
				PrintModelId = printModel.Data.Id,
				SerialNumber = snmpResult.Data["SerialNumber"],
				
				LocationId = printer.LocationId,
				ApplicationUsers = new List<ApplicationUser>(){currentUser.Data}
			};

			var resultAdd = await _printRepo.InsertAsync(newPrinter);
			if (!resultAdd.IsSuccess) return ServiceResponse<bool>.Failure(resultAdd.Message);
		}
		else
		{
			if (printerExist.Data.ApplicationUsers!.Any(u => u.Id == printer.ApplicationUserId)) return ServiceResponse<bool>.Failure("У тебя уже есть этот принтер");
			printerExist.Data.ApplicationUsers!.Add(currentUser.Data);
			var resultUpdate = await _printRepo.UpdateAsync(printerExist.Data);
			if (!resultUpdate.IsSuccess) return ServiceResponse<bool>.Failure(resultUpdate.Message);
		}
		return ServiceResponse<bool>.Success(true, "Принтер добавлен");
	}
	
	public async Task<IServiceResponse<bool>> DeleteAsync(int id, int userId)
	{
		if (userId <= 0) return ServiceResponse<bool>.Failure("Неавторизованная операция");

		var printerExist = await _printRepo.GetByIdAsync(id);
		if (!printerExist.IsSuccess) return ServiceResponse<bool>.Failure(printerExist.Message);

		printerExist.Data.ApplicationUsers!.Remove(printerExist.Data.ApplicationUsers.FirstOrDefault(u => u.Id == userId));

		var removePrinterFromUser = await _printRepo.UpdateAsync(printerExist.Data);
		if (!removePrinterFromUser.IsSuccess) return ServiceResponse<bool>.Failure(removePrinterFromUser.Message);

		if (removePrinterFromUser.Data.ApplicationUsers!.Count >= 1) return ServiceResponse<bool>.Success(true, "Принтер удален");
		else
		{
			var resultDeletePrinter = await _printRepo.DeleteByIdAsync(id);
			if (!resultDeletePrinter.IsSuccess) return ServiceResponse<bool>.Failure(resultDeletePrinter.Message);
			return ServiceResponse<bool>.Success(true, resultDeletePrinter.Message);
		}
	}

	public async Task<IServiceResponse<List<Printer>>> GetAllAsync()
	{
		var printers = await _printRepo.GetAllAsync();
		if (!printers.IsSuccess) return ServiceResponse<List<Printer>>.Failure(printers.Message);
		return ServiceResponse<List<Printer>>.Success(printers.Data, "Принтеры получены");
	}

	public async Task<IServiceResponse<List<Printer>>> GetAllByLocationAsync(int locationId, int userId)
	{
		// if (userId <= 0) return ServiceResponse<List<Printer>>.Failure("Неавторизованная операция");
		var printers = await _printRepo.GetAllByLocationAsync(locationId);
		if (!printers.IsSuccess) return ServiceResponse<List<Printer>>.Failure(printers.Message);
		return ServiceResponse<List<Printer>>.Success(printers.Data, "Принтеры получены");
	}

	public async Task<IServiceResponse<List<Printer>>> GetAllByModelAsync(int modelId, int userId)
	{
		// if (string.IsNullOrEmpty(identityUserId)) return ServiceResponse<IEnumerable<PrinterDTO>>.Failure("Неавторизованная операция");
		var printers = await _printRepo.GetAllByModelAsync(modelId);
		if (!printers.IsSuccess) return ServiceResponse<List<Printer>>.Failure(printers.Message);
		return ServiceResponse<List<Printer>>.Success(printers.Data, "Принтеры получены");
	}

	public async Task<IServiceResponse<List<Printer>>> GetAllByUserAsync(int userId)
	{
		var printers = await _printRepo.GetAllByUserAsync(userId);
		if (!printers.IsSuccess) return ServiceResponse<List<Printer>>.Failure(printers.Message);
		return ServiceResponse<List<Printer>>.Success(printers.Data, printers.Message);
	}

	public async Task<IServiceResponse<Printer>> GetByIdAsync(int id)
	{
		var printer = await _printRepo.GetByIdAsync(id);
		if (!printer.IsSuccess) return ServiceResponse<Printer>.Failure(printer.Message);
		return ServiceResponse<Printer>.Success(printer.Data, printer.Message);
	}

	public async Task<IServiceResponse<PrinterDetailDTO>> GetDetailByIdAsync(int id, int userId)
	{
		var printer = await _printRepo.GetByIdAsync(id);
		if (!printer.IsSuccess) return ServiceResponse<PrinterDetailDTO>.Failure(printer.Message);
		var oids = await _consumableService.GetAllByModelAsync(printer.Data.PrintModelId);
		if (!oids.IsSuccess) return ServiceResponse<PrinterDetailDTO>.Failure(oids.Message);

		var oidsForSNMP = new List<Variable>();
		foreach (var oid in oids.Data)
		{
			oidsForSNMP.Add(new(new ObjectIdentifier(oid.PrintOid!.Value!)));
		}
		var oidsResult = await _snmpService.GetOidsAsync(printer.Data.IpAddress, oidsForSNMP);
		if (!oidsResult.IsSuccess) return ServiceResponse<PrinterDetailDTO>.Failure(oidsResult.Message);
		var oidDict = oids.Data.ToDictionary(o => o.PrintOid!.Value!, o => o.Name!);
		var oidsDTO = new List<PrintOidDTO>();
		foreach (var result in oidsResult.Data)
		{
			if (oidDict.TryGetValue(result.Id.ToString(), out var name))
			{
				oidsDTO.Add(new PrintOidDTO()
				{
					Name = name,
					Value = result.Id.ToString(),
					Result = result.Data.ToString()
				});
			}
		}
		var detailPrinter = new PrinterDetailDTO()
		{
			Id = printer.Data.Id,
			Name = printer.Data.Name,
			IpAddress = printer.Data.IpAddress,
			Location = printer.Data.Location!.Name,
			PrintModel = printer.Data.PrintModel!.Name,
			PrintConsumables = oidsDTO
		};
		return ServiceResponse<PrinterDetailDTO>.Success(detailPrinter, "Данные получены");
	}
	public async Task<IServiceResponse<Printer>> UpdateAsync(UpdatePrinterDTO model, int userId)
	{
		var printerExist = await _printRepo.GetByIdAsync(model.Id);
		if (printerExist.Errors.Any()) return ServiceResponse<Printer>.Error(printerExist.Message);
		if (!printerExist.IsSuccess) return ServiceResponse<Printer>.Failure(printerExist.Message);
		
		printerExist.Data.Name = model.Name;
		printerExist.Data.LocationId = model.LocationId;
		printerExist.Data.IpAddress = model.IpAddress!;
		var resultUpdate = await _printRepo.UpdateAsync(printerExist.Data);
		
		if (!resultUpdate.IsSuccess) return ServiceResponse<Printer>.Error(resultUpdate.Message);
		return ServiceResponse<Printer>.Success(resultUpdate.Data, "Принтер обновлен");
	}
}
