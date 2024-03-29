﻿namespace PrintStatus.BLL.Services.Implementations;

using AutoMapper;
using DOM.Models;
using DTO;
using Helpers;
using Interfaces;
using Lextm.SharpSnmpLib;

public class BasePrinterManagementService(
										IPrinterRepository printRepo,
										IPrintModelManagementService modelService,
										ILocationManagementService locationService,
										IPrintOidManagementService oidService,
										IPrinterUsersRepository printerUsersRepo,
										ISnmpService snmpService,
										IMapper mapper
										// IAccountService accountService
										) : IPrinterManagementService
{
	private readonly IPrinterRepository _printRepo = printRepo;
	private readonly IPrintModelManagementService _modelService = modelService;
	private readonly ILocationManagementService _locationService = locationService;
	private readonly IPrintOidManagementService _oidService = oidService;
	private readonly IPrinterUsersRepository _printerUsersRepo = printerUsersRepo;
	private readonly ISnmpService _snmpService = snmpService;
	private readonly IMapper _mapper = mapper;
	// private readonly IAccountService _accountService = accountService;

	public async Task<IServiceResponse<PrinterDTO>> AddAsync(NewPrinterDTO printer)
	{
		if (printer == null) return ServiceResponse<PrinterDTO>.Failure("Неверный идентификатор принтера");



		var snmpResult = await _snmpService.GetModelAndSerialNumAsync(printer.IpAddress);
		if (!snmpResult.IsSuccess) return ServiceResponse<PrinterDTO>.Failure(snmpResult.Message);


		var printerExist = await _printRepo.GetBySerialNumberAsync(snmpResult.Data["SerialNumber"]);
		if (printerExist.Errors.Any()) return ServiceResponse<PrinterDTO>.Failure(printerExist.Message);

		PrinterDTO newPrinterDTO;
		if (!printerExist.IsSuccess)
		{
			var printModel = await _modelService.AddAsync(snmpResult.Data["Model"]);
			if (!printModel.IsSuccess) return ServiceResponse<PrinterDTO>.Failure(printModel.Message);

			var newPrinter = new Printer()
			{
				IpAddress = printer.IpAddress,
				Title = printer.Title,
				PrintModelId = printModel.Data.Id,
				SerialNumber = snmpResult.Data["SerialNumber"],
				LocationId = printer.LocationId
			};

			var resultAdd = await _printRepo.AddAsync(newPrinter);
			if (!resultAdd.IsSuccess) return ServiceResponse<PrinterDTO>.Failure(resultAdd.Message);

			await _printerUsersRepo.AddPrinterForUserAsync(printer.ApplicationUserId, resultAdd.Data.Id);

			newPrinterDTO = _mapper.Map<PrinterDTO>(resultAdd.Data);
		}
		else
		{
			if (printerExist.Data.PrinterUsers.Where(u => u.UserId.Equals(printer.ApplicationUserId)).Any()) return ServiceResponse<PrinterDTO>.Failure("Проверь свое зрение. Этот принтер у тебя уже есть!");
			printerExist.Data.PrinterUsers.Add(new BasePrinterUser { BasePrinterId = printerExist.Data.Id, UserId = printer.ApplicationUserId });
			var resultUpdate = await _printRepo.UpdateAsync(printerExist.Data);
			if (!resultUpdate.IsSuccess) return ServiceResponse<PrinterDTO>.Failure(resultUpdate.Message);
			newPrinterDTO = _mapper.Map<PrinterDTO>(resultUpdate.Data);
		}
		return ServiceResponse<PrinterDTO>.Success(newPrinterDTO, "Принтер добавлен");
	}

	/// <summary>
	/// Deletes a printer by its ID.
	/// </summary>
	/// <param name="id">The ID of the printer to delete.</param>
	/// <param name="identityUserId">The identity user ID.</param>
	/// <returns>A task that represents the asynchronous operation. The task result contains the service result indicating whether the printer was deleted successfully.</returns>
	public async Task<IServiceResponse<bool>> DeleteAsync(int id, string identityUserId)
	{
		if (id <= 0) return ServiceResponse<bool>.Failure("Неверный идентификатор притентера");
		if (string.IsNullOrEmpty(identityUserId)) return ServiceResponse<bool>.Failure("Неавторизованная операция");

		var printerExist = await _printRepo.GetByIdAsync(id);
		if (!printerExist.IsSuccess) return ServiceResponse<bool>.Failure(printerExist.Message);

		printerExist.Data.PrinterUsers.Remove(printerExist.Data.PrinterUsers.FirstOrDefault(u => u.UserId.Equals(identityUserId)));

		var removePrinterFromUser = await _printRepo.UpdateAsync(printerExist.Data);
		if (!removePrinterFromUser.IsSuccess) return ServiceResponse<bool>.Failure(removePrinterFromUser.Message);

		if (removePrinterFromUser.Data.PrinterUsers.Count >= 1) return ServiceResponse<bool>.Success(true, "Принтер удален");
		else
		{
			var resultDeletePrinter = await _printRepo.DeleteAsync(id);
			if (!resultDeletePrinter.IsSuccess) return ServiceResponse<bool>.Failure(resultDeletePrinter.Message);
			return ServiceResponse<bool>.Success(true, resultDeletePrinter.Message);
		}
	}

	public async Task<IServiceResponse<IEnumerable<PrinterDTO>>> GetAllAsync()
	{
		var printers = await _printRepo.GetAllAsync();
		if (!printers.IsSuccess) return ServiceResponse<IEnumerable<PrinterDTO>>.Failure(printers.Message);
		var result = new List<PrinterDTO>();
		foreach (var printer in printers.Data)
		{
			result.Add(_mapper.Map<PrinterDTO>(printer));
		}
		return ServiceResponse<IEnumerable<PrinterDTO>>.Success(result, "Принтеры получены");
	}

	public async Task<IServiceResponse<IEnumerable<PrinterDTO>>> GetAllByLocationAsync(int locationId, string identityUserId)
	{
		if (locationId <= 0) return ServiceResponse<IEnumerable<PrinterDTO>>.Failure("Неверный идентификатор местоположения");
		if (string.IsNullOrEmpty(identityUserId)) return ServiceResponse<IEnumerable<PrinterDTO>>.Failure("Неавторизованная операция");
		var printers = await _printRepo.GetAllByLocationAsync(locationId, identityUserId);
		if (!printers.IsSuccess) return ServiceResponse<IEnumerable<PrinterDTO>>.Failure(printers.Message);
		var result = new List<PrinterDTO>();
		foreach (var printer in printers.Data)
		{
			result.Add(_mapper.Map<PrinterDTO>(printer));
		}
		return ServiceResponse<IEnumerable<PrinterDTO>>.Success(result, "Принтеры получены");
	}

	public async Task<IServiceResponse<IEnumerable<PrinterDTO>>> GetAllByModelAsync(int modelId, string identityUserId)
	{
		if (modelId <= 0) return ServiceResponse<IEnumerable<PrinterDTO>>.Failure("Неверный идентификатор модели");
		if (string.IsNullOrEmpty(identityUserId)) return ServiceResponse<IEnumerable<PrinterDTO>>.Failure("Неавторизованная операция");
		var printers = await _printRepo.GetAllByModelAsync(modelId, identityUserId);
		if (!printers.IsSuccess) return ServiceResponse<IEnumerable<PrinterDTO>>.Failure(printers.Message);
		var result = new List<PrinterDTO>();
		foreach (var printer in printers.Data)
		{
			result.Add(_mapper.Map<PrinterDTO>(printer));
		}
		return ServiceResponse<IEnumerable<PrinterDTO>>.Success(result, "Принтеры получены");
	}

	public async Task<IServiceResponse<IEnumerable<PrinterDTO>>> GetAllByUserAsync(string identityUserId)
	{
		if (string.IsNullOrEmpty(identityUserId)) return ServiceResponse<IEnumerable<PrinterDTO>>.Failure("Неавторизованная операция");
		var printers = await _printRepo.GetAllByUserAsync(identityUserId);
		if (!printers.IsSuccess) return ServiceResponse<IEnumerable<PrinterDTO>>.Failure(printers.Message);
		var result = new List<PrinterDTO>();
		foreach (var printer in printers.Data)
		{
			result.Add(_mapper.Map<PrinterDTO>(printer));
		}
		return ServiceResponse<IEnumerable<PrinterDTO>>.Success(result, printers.Message);
	}

	public async Task<IServiceResponse<PrinterDTO>> GetByIdAsync(int id)
	{
		if (id <= 0) return ServiceResponse<PrinterDTO>.Failure("Неверный идентификатор притентера");
		var printer = await _printRepo.GetByIdAsync(id);
		if (!printer.IsSuccess) return ServiceResponse<PrinterDTO>.Failure(printer.Message);
		var result = _mapper.Map<PrinterDTO>(printer.Data);
		return ServiceResponse<PrinterDTO>.Success(result, printer.Message);
	}

	public async Task<IServiceResponse<PrinterDetailDTO>> GetDetailByIdAsync(int id, string identityUserId)
	{
		if (id <= 0) return ServiceResponse<PrinterDetailDTO>.Failure("Неверный идентификатор принтера");
		if (string.IsNullOrEmpty(identityUserId)) return ServiceResponse<PrinterDetailDTO>.Failure("Неавторизованная операция");
		var printer = await _printRepo.GetByIdAsync(id);
		if (!printer.IsSuccess) return ServiceResponse<PrinterDetailDTO>.Failure(printer.Message);
		var oids = await _oidService.GetAllByModelAsync(printer.Data.PrintModelId);
		if (!oids.IsSuccess) return ServiceResponse<PrinterDetailDTO>.Failure(oids.Message);
		var printModel = await _modelService.GetByIdAsync(printer.Data.PrintModelId);
		if (!printModel.IsSuccess) return ServiceResponse<PrinterDetailDTO>.Failure(printModel.Message);

		var oidsForSNMP = new List<Variable>();
		foreach (var oid in oids.Data)
		{
			oidsForSNMP.Add(new(new ObjectIdentifier(oid.Value)));
		}
		var oidsResult = await _snmpService.GetOidsAsync(printer.Data.IpAddress, oidsForSNMP);
		if (!oidsResult.IsSuccess) return ServiceResponse<PrinterDetailDTO>.Failure(oidsResult.Message);
		var oidDict = oids.Data.ToDictionary(o => o.Value, o => o.Title);
		var oidsDTO = new List<OidDTO>();
		foreach (var result in oidsResult.Data)
		{
			if (oidDict.TryGetValue(result.Id.ToString(), out var title))
			{
				oidsDTO.Add(new OidDTO()
				{
					Title = title,
					Value = result.Id.ToString(),
					Result = result.Data.ToString()
				});
			}
		}
		var detailPrinter = new PrinterDetailDTO()
		{
			Id = printer.Data.Id,
			Title = printer.Data.Title,
			IpAddress = printer.Data.IpAddress,
			LocationId = printer.Data.LocationId,
			PrintModelId = printer.Data.PrintModelId,
			Location = printer.Data.Location.Title,
			PrintModel = printModel.Data.Title,
			PrintConsumables = oidsDTO
		};
		return ServiceResponse<PrinterDetailDTO>.Success(detailPrinter, "Данные получены");
	}
	public async Task<IServiceResponse<PrinterDTO>> UpdateAsync(PrinterDTO printerDTO, string identityUserId)
	{
		if (printerDTO == null) return ServiceResponse<PrinterDTO>.Failure("Неверный идентификатор принтера");
		if (string.IsNullOrEmpty(identityUserId)) return ServiceResponse<PrinterDTO>.Failure("Неавторизованная операция");
		//var userRoles = await _accountService.GetRolesAsync(identityUserId);
		//if (!userRoles.IsSuccess) return ServiceResponse<PrinterDTO>.Failure("Неудалось получить роль пользователя");
		//if (!userRoles.Data.Any(r => r.Equals("Администратор"))) return ServiceResponse<PrinterDTO>.Failure("Недостаточно прав для обновления принтера");
		Printer printer = _mapper.Map<Printer>(printerDTO);
		var resultUpdate = await _printRepo.UpdateAsync(printer);
		if (!resultUpdate.IsSuccess) return ServiceResponse<PrinterDTO>.Failure(resultUpdate.Message);
		printerDTO = _mapper.Map<PrinterDTO>(resultUpdate.Data);
		return ServiceResponse<PrinterDTO>.Success(printerDTO, "Принтер обновлен");
	}
}
