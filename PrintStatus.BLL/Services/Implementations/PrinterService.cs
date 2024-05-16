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
		if (currentUser.Data == null) return ServiceResponse<bool>.Error("Пользователь не найден");

		var snmpResult = await _snmpService.GetModelAndSerialNumAsync(printer.IpAddress);
		if (!snmpResult.IsSuccess) return ServiceResponse<bool>.Error(snmpResult.Message);

		var printerExist = await _printRepo.GetBySerialNumberAsync(snmpResult.Data["SerialNumber"]);
		if (printerExist.Errors.Any()) return ServiceResponse<bool>.Error(printerExist.Message);

		if (!printerExist.IsSuccess)
		{
			var printModel = await _modelService.InsertAsync(snmpResult.Data["Model"]);
			if (printModel.HasErrors || !printModel.IsSuccess) return ServiceResponse<bool>.Error(printModel.Message);

			var newPrinter = new Printer
			{
				IpAddress = printer.IpAddress,
				Name = printer.Name,
				PrintModelId = printModel.Data.Id,
				SerialNumber = snmpResult.Data["SerialNumber"],
				LocationId = printer.LocationId,
				ApplicationUsers = new List<ApplicationUser> { currentUser.Data }
			};

			var resultAdd = await _printRepo.InsertAsync(newPrinter);
			if (resultAdd.Errors.Any() || !resultAdd.IsSuccess) return ServiceResponse<bool>.Error(resultAdd.Message);
		}
		else
		{
			if (printerExist.Data.ApplicationUsers.Any(u => u.Id == userId)) return ServiceResponse<bool>.Failure("У тебя уже есть этот принтер");

			printerExist.Data.ApplicationUsers.Add(currentUser.Data);
			var resultUpdate = await _printRepo.UpdateAsync(printerExist.Data);
			if (resultUpdate.Errors.Any() || !resultUpdate.IsSuccess) return ServiceResponse<bool>.Error(resultUpdate.Message);
		}

		return ServiceResponse<bool>.Success(true, "Принтер добавлен");
	}

	
	public async Task<IServiceResponse<bool>> DeleteAsync(int id, int userId)
	{
		var printerExist = await _printRepo.GetByIdAsync(id);
		if (printerExist.Errors.Any()) 
			return ServiceResponse<bool>.Error(printerExist.Message);
		if (!printerExist.IsSuccess)
			return ServiceResponse<bool>.Failure(printerExist.Message);

		if (printerExist.Data.ApplicationUsers!.Find(u => u.Id == userId) == null)
			return ServiceResponse<bool>.Failure("У тебя нет этого принтера");

		printerExist.Data.ApplicationUsers!.Remove(printerExist.Data.ApplicationUsers.Find(u => u.Id == userId));
		var removePrinterFromUser = await _printRepo.UpdateAsync(printerExist.Data);

		if (removePrinterFromUser.Errors.Any()) 
			return ServiceResponse<bool>.Error(removePrinterFromUser.Message);
		if (!removePrinterFromUser.IsSuccess) 
			return ServiceResponse<bool>.Failure(removePrinterFromUser.Message);
		if (removePrinterFromUser.Data.ApplicationUsers!.Count >= 1) 
			return ServiceResponse<bool>.Success(true, "Принтер удален");
		
		var resultDeletePrinter = await _printRepo.DeleteByIdAsync(id);
		if (resultDeletePrinter.Errors.Any())
			return ServiceResponse<bool>.Error(resultDeletePrinter.Message);
		if (!resultDeletePrinter.IsSuccess) 
			return ServiceResponse<bool>.Failure(resultDeletePrinter.Message);
		return ServiceResponse<bool>.Success(true, resultDeletePrinter.Message);
	}

	public async Task<IServiceResponse<List<Printer>>> GetAllAsync()
	{
		var printers = await _printRepo.GetAllAsync();
		if (!printers.IsSuccess) 
			return ServiceResponse<List<Printer>>.Error(printers.Message);
		return ServiceResponse<List<Printer>>.Success(printers.Data, "Принтеры получены");
	}

	public async Task<IServiceResponse<List<Printer>>> GetAllByLocationAsync(int locationId, int userId)
	{
		var printers = await _printRepo.GetAllByLocationAsync(locationId);
		if (!printers.IsSuccess) 
			return ServiceResponse<List<Printer>>.Error(printers.Message);
		return ServiceResponse<List<Printer>>.Success(printers.Data, "Принтеры получены");
	}

	public async Task<IServiceResponse<List<Printer>>> GetAllByModelAsync(int modelId, int userId)
	{
		var printers = await _printRepo.GetAllByModelAsync(modelId);
		if (!printers.IsSuccess) 
			return ServiceResponse<List<Printer>>.Error(printers.Message);
		return ServiceResponse<List<Printer>>.Success(printers.Data, "Принтеры получены");
	}

	public async Task<IServiceResponse<List<Printer>>> GetAllByUserAsync(int userId)
	{
		var printers = await _printRepo.GetAllByUserAsync(userId);
		if (!printers.IsSuccess) 
			return ServiceResponse<List<Printer>>.Error(printers.Message);
		return ServiceResponse<List<Printer>>.Success(printers.Data, printers.Message);
	}

	public async Task<IServiceResponse<Printer>> GetByIdAsync(int id)
	{
		var printer = await _printRepo.GetByIdAsync(id);
		if(printer.Errors.Any()) return ServiceResponse<Printer>.Error(printer.Message);
		if (!printer.IsSuccess) return ServiceResponse<Printer>.Failure(printer.Message);
		return ServiceResponse<Printer>.Success(printer.Data, printer.Message);
	}

	public async Task<IServiceResponse<PrinterDetailDTO>> GetDetailByIdAsync(int id)
	{
		var printer = await _printRepo.GetByIdAsync(id);
		if (printer.Errors.Any()) return ServiceResponse<PrinterDetailDTO>.Error(printer.Message);
		if (!printer.IsSuccess) return ServiceResponse<PrinterDetailDTO>.Failure(printer.Message);
		
		var oids = await _consumableService.GetAllByModelAsync(printer.Data.PrintModelId);
		if(oids.HasErrors) ServiceResponse<PrinterDetailDTO>.Error(oids.Message);
		if (!oids.IsSuccess) return ServiceResponse<PrinterDetailDTO>.Failure(oids.Message);

		var oidsForSNMP = new List<Variable>();
		foreach (var oid in oids.Data)
		{
			oidsForSNMP.Add(new(new ObjectIdentifier(oid.PrintOid!.Value!)));
		}
		var oidsResult = await _snmpService.GetOidsAsync(printer.Data.IpAddress, oidsForSNMP);
		if (oidsResult.HasErrors) return ServiceResponse<PrinterDetailDTO>.Error(oidsResult.Message);
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
