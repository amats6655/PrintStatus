using AutoMapper;
using Lextm.SharpSnmpLib;
using PrintStatus.BLL.DTO;
using PrintStatus.BLL.Interfaces;
using PrintStatus.DOM.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.BLL.Services
{
	public class BasePrinterManagementService(
                                            IBasePrinterRepository printRepo,
                                            IPrintModelManagementService modelService,
                                            ILocationManagementService locationService,
                                            IPrintOidManagementService oidService,
                                            IUserProfileRepository profileRepo,
                                            ISnmpService snmpService,
                                            IMapper mapper,
                                            IAccountService accountService
                                            ) : IBasePrinterManagementService
	{
		private readonly IBasePrinterRepository _printRepo = printRepo;
		private readonly IPrintModelManagementService _modelService = modelService;
		private readonly ILocationManagementService _locationService = locationService;
		private readonly IPrintOidManagementService _oidService = oidService;
		private readonly IUserProfileRepository _profileRepo = profileRepo;
		private readonly ISnmpService _snmpService = snmpService;
		private readonly IMapper _mapper = mapper;
		private readonly IAccountService _accountService = accountService;

        public async Task<IServiceResult<PrinterDTO>> AddAsync(NewPrinterDTO printer)
		{
			if (printer == null) return ServiceResult<PrinterDTO>.Failure("Неверный идентификатор принтера");
			var snmpResult = await _snmpService.GetModelAndSerialNumAsync(printer.IpAddress);
			if (!snmpResult.IsSuccess) return ServiceResult<PrinterDTO>.Failure(snmpResult.Message);
			var userProfile = await _profileRepo.GetUserByIdentityId(printer.IdentityUserId);
			if (!userProfile.IsSuccess) return ServiceResult<PrinterDTO>.Failure(userProfile.Message);
			var printerExist = await _printRepo.GetBySerialNumberAsync(snmpResult.Data["SerialNumber"]);
			if (printerExist.Errors.Any()) return ServiceResult<PrinterDTO>.Failure(printerExist.Message);
			PrinterDTO newPrinterDTO;
			if (!printerExist.IsSuccess)
			{
				var printModel = await _modelService.AddAsync(snmpResult.Data["Model"]);
				if (!printModel.IsSuccess) return ServiceResult<PrinterDTO>.Failure(printModel.Message);
				var newPrinter = new BasePrinter()
				{
					IpAddress = printer.IpAddress,
					Title = printer.Title,
					PrintModelId = printModel.Data.Id,
					SerialNumber = snmpResult.Data["SerialNumber"],
					LocationId = printer.LocationId,
					UserProfiles = new List<UserProfile> { userProfile.Data },
				};
				var resultAdd = await _printRepo.AddAsync(newPrinter);
				if (!resultAdd.IsSuccess) return ServiceResult<PrinterDTO>.Failure(resultAdd.Message);
				newPrinterDTO = _mapper.Map<PrinterDTO>(resultAdd.Data);
			}
			else
			{
				if (printerExist.Data.UserProfiles.Where(u => u.Id == userProfile.Data.Id).Any()) return ServiceResult<PrinterDTO>.Failure("Проверь свое зрение. Этот принтер у тебя уже есть!");
				printerExist.Data.UserProfiles.Add(userProfile.Data);
				var resultUpdate = await _printRepo.UpdateAsync(printerExist.Data);
				if (!resultUpdate.IsSuccess) return ServiceResult<PrinterDTO>.Failure(resultUpdate.Message);
				newPrinterDTO = _mapper.Map<PrinterDTO>(resultUpdate.Data);
			}
			return ServiceResult<PrinterDTO>.Success(newPrinterDTO, "Принтер добавлен");
		}

		public async Task<IServiceResult<bool>> DeleteAsync(int id, string identityUserId)
		{
			if (id <= 0) return ServiceResult<bool>.Failure("Неверный идентификатор притентера");
			if (string.IsNullOrEmpty(identityUserId)) return ServiceResult<bool>.Failure("Неавторизованная операция");
			var printerExist = await _printRepo.GetByIdAsync(id);
			if (!printerExist.IsSuccess) return ServiceResult<bool>.Failure(printerExist.Message);
			var userProfile = await _profileRepo.GetUserByIdentityId(identityUserId);
			if (!userProfile.IsSuccess) return ServiceResult<bool>.Failure(userProfile.Message);

			printerExist.Data.UserProfiles.Remove(userProfile.Data);
			var removePrinterFromUser = await _printRepo.UpdateAsync(printerExist.Data);
			if (!removePrinterFromUser.IsSuccess) return ServiceResult<bool>.Failure(removePrinterFromUser.Message);
			if (removePrinterFromUser.Data.UserProfiles.Count >= 1) return ServiceResult<bool>.Success(true, "Принтер удален");
			else
			{
				var resultDeletePrinter = await _printRepo.DeleteAsync(id);
				if (!resultDeletePrinter.IsSuccess) return ServiceResult<bool>.Failure(resultDeletePrinter.Message);
				return ServiceResult<bool>.Success(true, resultDeletePrinter.Message);
			}
		}

		public async Task<IServiceResult<IEnumerable<PrinterDTO>>> GetAllAsync()
		{
			var printers = await _printRepo.GetAllAsync();
			if (!printers.IsSuccess) return ServiceResult<IEnumerable<PrinterDTO>>.Failure(printers.Message);
			var result = new List<PrinterDTO>();
			foreach (var printer in printers.Data)
			{
				result.Add(_mapper.Map<PrinterDTO>(printer));
			}
			return ServiceResult<IEnumerable<PrinterDTO>>.Success(result, "Принтеры получены");
		}

		public async Task<IServiceResult<IEnumerable<PrinterDTO>>> GetAllByLocationAsync(int locationId, string identityUserId)
		{
			if (locationId <= 0) return ServiceResult<IEnumerable<PrinterDTO>>.Failure("Неверный идентификатор местоположения");
			if (string.IsNullOrEmpty(identityUserId)) return ServiceResult<IEnumerable<PrinterDTO>>.Failure("Неавторизованная операция");
			var printers = await _printRepo.GetAllByLocationAsync(locationId, identityUserId);
			if (!printers.IsSuccess) return ServiceResult<IEnumerable<PrinterDTO>>.Failure(printers.Message);
			var result = new List<PrinterDTO>();
			foreach (var printer in printers.Data)
			{
				result.Add(_mapper.Map<PrinterDTO>(printer));
			}
			return ServiceResult<IEnumerable<PrinterDTO>>.Success(result, "Принтеры получены");
		}

		public async Task<IServiceResult<IEnumerable<PrinterDTO>>> GetAllByModelAsync(int modelId, string identityUserId)
		{
			if (modelId <= 0) return ServiceResult<IEnumerable<PrinterDTO>>.Failure("Неверный идентификатор модели");
			if (string.IsNullOrEmpty(identityUserId)) return ServiceResult<IEnumerable<PrinterDTO>>.Failure("Неавторизованная операция");
			var printers = await _printRepo.GetAllByModelAsync(modelId, identityUserId);
			if (!printers.IsSuccess) return ServiceResult<IEnumerable<PrinterDTO>>.Failure(printers.Message);
			var result = new List<PrinterDTO>();
			foreach (var printer in printers.Data)
			{
				result.Add(_mapper.Map<PrinterDTO>(printer));
			}
			return ServiceResult<IEnumerable<PrinterDTO>>.Success(result, "Принтеры получены");
		}

		public async Task<IServiceResult<IEnumerable<PrinterDTO>>> GetAllByUserAsync(string identityUserId)
		{
			if (string.IsNullOrEmpty(identityUserId)) return ServiceResult<IEnumerable<PrinterDTO>>.Failure("Неавторизованная операция");
			var printers = await _printRepo.GetAllByUserAsync(identityUserId);
			if (!printers.IsSuccess) return ServiceResult<IEnumerable<PrinterDTO>>.Failure(printers.Message);
			var result = new List<PrinterDTO>();
			foreach (var printer in printers.Data)
			{
				result.Add(_mapper.Map<PrinterDTO>(printer));
			}
			return ServiceResult<IEnumerable<PrinterDTO>>.Success(result, printers.Message);
		}

		public async Task<IServiceResult<PrinterDTO>> GetByIdAsync(int id)
		{
			if (id <= 0) return ServiceResult<PrinterDTO>.Failure("Неверный идентификатор притентера");
			var printer = await _printRepo.GetByIdAsync(id);
			if (!printer.IsSuccess) return ServiceResult<PrinterDTO>.Failure(printer.Message);
			var result = _mapper.Map<PrinterDTO>(printer);
			return ServiceResult<PrinterDTO>.Success(result, printer.Message);
		}

		public async Task<IServiceResult<PrinterDetailDTO>> GetDetailByIdAsync(int id, string identityUserId)
		{
			if (id <= 0) return ServiceResult<PrinterDetailDTO>.Failure("Неверный идентификатор принтера");
			if (string.IsNullOrEmpty(identityUserId)) return ServiceResult<PrinterDetailDTO>.Failure("Неавторизованная операция");
			var printer = await GetByIdAsync(id);
			if (!printer.IsSuccess) return ServiceResult<PrinterDetailDTO>.Failure(printer.Message);
			var oids = await _oidService.GetAllByModelAsync(printer.Data.ModelId);
			if (!oids.IsSuccess) return ServiceResult<PrinterDetailDTO>.Failure(oids.Message);
			var location = await _locationService.GetByIdAsync(printer.Data.LocationId);
			if (!location.IsSuccess) return ServiceResult<PrinterDetailDTO>.Failure(location.Message);
			var printModel = await _modelService.GetByIdAsync(printer.Data.ModelId);
			if (!printModel.IsSuccess) return ServiceResult<PrinterDetailDTO>.Failure(printModel.Message);

			var oidsForSNMP = new List<Variable>();
			foreach (var oid in oids.Data)
			{
				oidsForSNMP.Add(new(new ObjectIdentifier(oid.Value)));
			}
			var oidsResult = await _snmpService.GetOidsAsync(printer.Data.IpAddress, oidsForSNMP);
			if (!oidsResult.IsSuccess) return ServiceResult<PrinterDetailDTO>.Failure(oidsResult.Message);
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
						Result = result.Data.ToString(),
					});
				}
			}
			var detailPrinter = new PrinterDetailDTO()
			{
				Id = printer.Data.Id,
				Title = printer.Data.Title,
				IpAddress = printer.Data.IpAddress,
				LocationId = printer.Data.LocationId,
				ModelId = printer.Data.ModelId,
				Location = location.Data.Title,
				Model = printModel.Data.Title,
				PrintConsumables = oidsDTO
			};
			return ServiceResult<PrinterDetailDTO>.Success(detailPrinter, "Данные получены");
		}
		public async Task<IServiceResult<PrinterDTO>> UpdateAsync(PrinterDTO printerDTO, string identityUserId)
		{
			if (printerDTO == null) return ServiceResult<PrinterDTO>.Failure("Неверный идентификатор принтера");
			if (string.IsNullOrEmpty(identityUserId)) return ServiceResult<PrinterDTO>.Failure("Неавторизованная операция");
			var userRoles = await _accountService.GetRolesAsync(identityUserId);
			if (!userRoles.IsSuccess) return ServiceResult<PrinterDTO>.Failure("Неудалось получить роль пользователя");
			if (!userRoles.Data.Any(r => r.Equals("Администратор"))) return ServiceResult<PrinterDTO>.Failure("Недостаточно прав для обновления принтера");
			BasePrinter printer = _mapper.Map<BasePrinter>(printerDTO);
			var resultUpdate = await _printRepo.UpdateAsync(printer);
			if (!resultUpdate.IsSuccess) return ServiceResult<PrinterDTO>.Failure(resultUpdate.Message);
			printerDTO = _mapper.Map<PrinterDTO>(resultUpdate.Data);
			return ServiceResult<PrinterDTO>.Success(printerDTO, "Принтер обновлен");
		}
	}

}
