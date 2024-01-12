using Lextm.SharpSnmpLib;
using Microsoft.AspNetCore.Http.HttpResults;
using PrintStatus.BLL.DTO;
using PrintStatus.BLL.Interfaces;
using PrintStatus.DOM.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.BLL.Services
{
	public class BasePrinterManagementService : IBasePrinterManagementService
	{
		private readonly IBasePrinterRepository _printRepo;
		private readonly IPrintModelRepository _modelRepo;
		private readonly ILocationRepository _locationRepo;
		private readonly IOidRepository _oidRepo;
		private readonly IUserProfileRepository _profileRepo;
		private readonly ISnmpService _snmpService;
		public BasePrinterManagementService(
												IBasePrinterRepository printRepo,
												IPrintModelRepository modelRepo,
												ILocationRepository locationRepo,
												IOidRepository oidRepo,
												IUserProfileRepository profileRepo,
												ISnmpService snmpService
											)
		{
			_printRepo = printRepo;
			_modelRepo = modelRepo;
			_locationRepo = locationRepo;
			_oidRepo = oidRepo;
			_profileRepo = profileRepo;
			_snmpService = snmpService;
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
					int modelId = await _modelRepo.GetIdByModelNameAsync(snmpResult["Model"]);
					if (modelId == 0)
					{
						var addModel = await _modelRepo.AddAsync(new PrintModel() { Title = snmpResult["Model"] });
						modelId = addModel.Id;
					}

					var newPrinter = new BasePrinter()
					{
						IpAddress = ipAddress,
						Title = title,
						PrintModelId = modelId,
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
			
			return new PrinterDTO(result.Id, result.Title, result.PrintModelId, result.IpAddress, result.LocationId);
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
				foreach(var printer in dataprinters)
				{
					result.Add(new PrinterDTO(printer.Id, printer.Title, printer.PrintModelId, printer.IpAddress, printer.LocationId));
				}
				return result;
			}
			catch(Exception ex)
			{
				//TODO Добавить обработчик ошибок
				Console.WriteLine(ex.Message);
				return result;
			}
		}

		public async Task<IEnumerable<PrinterDTO>> GetAllByLocationAsync(int locationId, string identityUserId)
		{
			ArgumentException.ThrowIfNullOrEmpty(identityUserId, nameof(identityUserId));
			var result = new List<PrinterDTO>();
			if(locationId == 0) return result;
			var dataPrinters = await _printRepo.GetAllByLocationAsync(locationId, identityUserId);
			if (!dataPrinters.Any()) return result;

			foreach(var printer in dataPrinters)
			{
				result.Add(new PrinterDTO(printer.Id, printer.Title, printer.PrintModelId, printer.IpAddress, printer.LocationId));
			}
			return result;
		}

		public async Task<IEnumerable<PrinterDTO>> GetAllByModelAsync(int modelId, string identityUserId)
		{
			ArgumentException.ThrowIfNullOrEmpty(identityUserId, nameof(identityUserId));
			var result = new List<PrinterDTO>();
			if(modelId == 0) return result;
			var dataPrinters = await _printRepo.GetAllByModelAsync(modelId, identityUserId);
			if (!dataPrinters.Any()) return result;
			foreach(var printer in dataPrinters)
			{
				result.Add(new PrinterDTO(printer.Id, printer.Title, printer.PrintModelId, printer.IpAddress, printer.LocationId));
			}
			return result;
		}

		public async Task<IEnumerable<PrinterDTO>> GetAllByUserAsync(string identityUserId)
		{
			ArgumentException.ThrowIfNullOrEmpty(identityUserId, nameof(identityUserId));
			var result = new List<PrinterDTO>();
			var dataPrinters = await _printRepo.GetAllByUserAsync(identityUserId);
			if(!dataPrinters.Any()) return result;
			foreach(var printer in dataPrinters)
			{
				result.Add(new PrinterDTO(printer.Id, printer.Title, printer.PrintModelId, printer.IpAddress, printer.LocationId));
			}
			return result;
		}

		public async Task<PrinterDTO> GetByIdAsync(int id, string identityUserId)
		{
			var printer = await _printRepo.GetByIdAsync(id);
			if(printer != null) return  new PrinterDTO(printer.Id, printer.Title, printer.PrintModelId, printer.PrintModel.Title, printer.IpAddress, printer.LocationId, printer.Location.Title);
			return null;
		}

		public async Task<PrinterDetailDTO> GetDetailByIdAsync(int id, string identityUserId)
		{
			// Получаем стандартные модели
			var printer = await _printRepo.GetByIdAsync(id);
			var model = await _modelRepo.GetByIdAsync(printer.PrintModelId);
			var location = await _locationRepo.GetByIdAsync(printer.LocationId);
			var oids = await _oidRepo.GetAllByModelIdAsync(printer.PrintModelId);

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
			var userProfile = await _profileRepo.GetUserByIdentityId(identityUserId);
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
	}
}
