using AutoMapper;
using Lextm.SharpSnmpLib;
using PrintStatus.BLL.DTO;
using PrintStatus.BLL.Interfaces;
using PrintStatus.DOM.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.BLL.Services
{
	public class BasePrinterManagementService : IBasePrinterManagementService
	{
		private readonly IBasePrinterRepository _printRepo;
		private readonly IPrintModelManagementService _modelService;
		private readonly ILocationManagementService _locationService;
		private readonly IPrintOidManagementService _oidService;
		private readonly IUserProfileRepository _profileRepo;
		private readonly ISnmpService _snmpService;
		private readonly IMapper _mapper;
		public BasePrinterManagementService(
												IBasePrinterRepository printRepo,
												IPrintModelManagementService modelService,
												ILocationManagementService locationService,
												IPrintOidManagementService oidService,
												IUserProfileRepository profileRepo,
												ISnmpService snmpService,
												IMapper mapper
											)
		{
			_printRepo = printRepo;
			_modelService = modelService;
			_locationService = locationService;
			_oidService = oidService;
			_profileRepo = profileRepo;
			_snmpService = snmpService;
			_mapper = mapper;
		}

		public async Task<PrinterDTO> AddAsync(string title, string ipAddress, int locationId, string identityUserId)
		{
			ArgumentException.ThrowIfNullOrEmpty(ipAddress, nameof(ipAddress));
			var snmpResult = await _snmpService.GetModelAndSerialNumAsync(ipAddress);
			var userProfile = await _profileRepo.GetUserByIdentityId(identityUserId);
			var printer = await _printRepo.GetIdBySerialNumberAsync(snmpResult["SerialNumber"]);
			BasePrinter result;
			// Если принтера нет в базе
			if (printer == null)
			{
				try
				{
					var printModel = await _modelService.AddAsync(snmpResult["Model"]);

					var newPrinter = new BasePrinter()
					{
						IpAddress = ipAddress,
						Title = title,
						PrintModelId = printModel.Id,
						SerialNumber = snmpResult["SerialNumber"],
						LocationId = locationId,
						UserProfiles = new List<UserProfile>() { userProfile },
					};
					result = await _printRepo.AddAsync(newPrinter);
					//TODO Залоггировать выполнение операции
				}
				catch (Exception ex)
				{
					//TODO Написать обработчик ошибок
					Console.WriteLine(ex.Message);
					return null;
				}
			}
			// Если принтер есть, просто добавляем ему пользователя
			else
			{
				try
				{
					printer.UserProfiles.Add(userProfile);
					//TODO Залоггировать выполнение операции
					result = await _printRepo.UpdateAsync(printer);
				}
				catch (Exception ex)
				{
					//TODO Написать обработчик ошибок
					Console.WriteLine(ex.Message);
					return null;
				}
			}

			return _mapper.Map<PrinterDTO>(result);
		}

		public async Task<bool> DeleteAsync(int id, string identityUserId)
		{
			try
			{
				var printer = await _printRepo.GetByIdAsync(id);
				//TODO Залоггировать выполнение операции
				var result = await _printRepo.DeleteAsync(printer);
				return result;
			}
			catch (Exception ex)
			{
				//TODO Написать обработчик ошибок
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		public async Task<IEnumerable<PrinterDTO>> GetAllAsync()
		{
			var result = new List<PrinterDTO>();
			try
			{
				var dataprinters = await _printRepo.GetAllAsync();
				foreach (var printer in dataprinters)
				{
					result.Add(_mapper.Map<PrinterDTO>(printer));
				}
				return result;
			}
			catch (Exception ex)
			{
				//TODO Добавить обработчик ошибок
				Console.WriteLine(ex.Message);
				return null;
			}
		}

		public async Task<IEnumerable<PrinterDTO>> GetAllByLocationAsync(int locationId, string identityUserId)
		{
			ArgumentException.ThrowIfNullOrEmpty(identityUserId, nameof(identityUserId));
			var result = new List<PrinterDTO>();
			if (locationId == 0) return null;
			var dataPrinters = await _printRepo.GetAllByLocationAsync(locationId, identityUserId);
			if (!dataPrinters.Any())
			{
				foreach (var printer in dataPrinters)
				{
					result.Add(_mapper.Map<PrinterDTO>(printer));
				}
				return result;
			}
			return null;
		}

		public async Task<IEnumerable<PrinterDTO>> GetAllByModelAsync(int modelId, string identityUserId)
		{
			ArgumentException.ThrowIfNullOrEmpty(identityUserId, nameof(identityUserId));
			var result = new List<PrinterDTO>();
			if (modelId == 0) return null;
			var dataPrinters = await _printRepo.GetAllByModelAsync(modelId, identityUserId);
			if (dataPrinters.Any())
			{
				foreach (var printer in dataPrinters)
				{
					result.Add(_mapper.Map<PrinterDTO>(printer));
				}
				return result;
			}
			return null;
		}

		public async Task<IEnumerable<PrinterDTO>> GetAllByUserAsync(string identityUserId)
		{
			ArgumentException.ThrowIfNullOrEmpty(identityUserId, nameof(identityUserId));
			var result = new List<PrinterDTO>();
			var dataPrinters = await _printRepo.GetAllByUserAsync(identityUserId);
			if (dataPrinters.Any())
			{
				foreach (var printer in dataPrinters)
				{
					result.Add(_mapper.Map<PrinterDTO>(printer));
				}
				return result;
			}
			return null;
		}

		public async Task<PrinterDTO> GetByIdAsync(int id, string identityUserId)
		{
			var printer = await _printRepo.GetByIdAsync(id);
			if (printer != null) return _mapper.Map<PrinterDTO>(printer);
			return null;
		}

		public async Task<PrinterDetailDTO> GetDetailByIdAsync(int id, string identityUserId)
		{
			// Получаем стандартные модели
			var printer = await _printRepo.GetByIdAsync(id);
			var model = await _modelService.GetByIdAsync(printer.PrintModelId);
			var location = await _locationService.GetByIdAsync(printer.LocationId);
			var oids = await _oidService.GetAllByModelAsync(printer.PrintModelId);

			// Формируем список oid для запроса SNMP
			var oidsForSNMP = new List<Variable>();
			foreach (var oid in oids)
			{
				oidsForSNMP.Add(new(new ObjectIdentifier(oid.Value)));
			}
			var oidsResult = await _snmpService.GetOidsAsync(printer.IpAddress, oidsForSNMP);
			// Получаем результаты по SNMP
			var oidsDTO = new List<OidDTO>();
			// Собираем OidDTO
			var oidDict = oids.ToDictionary(o => o.Value, o => o.Title);
			foreach (var result in oidsResult)
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
				Id = id,
				Title = printer.Title,
				IpAddress = printer.IpAddress,
				LocationId = printer.LocationId,
				ModelId = model.Id,
				Location = location.Title,
				Model = model.Title,
				PrintConsumables = oidsDTO
			};
			return detailPrinter;
		}

		public async Task<PrinterDTO> UpdateAsync(PrinterDTO printer, string identityUserId)
		{
			ArgumentNullException.ThrowIfNull(printer);

			var editPrinter = await _printRepo.GetByIdAsync(printer.Id);
			if (editPrinter != null)
			{
				editPrinter.Title = printer.Title;
				editPrinter.IpAddress = printer.IpAddress;
				editPrinter.PrintModelId = printer.ModelId;
				editPrinter.LocationId = printer.LocationId;
				//TODO Залоггировать выполнение операции
				try
				{
					await _printRepo.UpdateAsync(editPrinter);
					return printer;
				}
				catch (Exception ex)
				{
					//TODO Добавить обработку ошибок
					Console.WriteLine(ex.Message);
					return null;
				}
			}
			return null;
		}
	}
}
