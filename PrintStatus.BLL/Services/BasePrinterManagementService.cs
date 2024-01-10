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
        private readonly IPrintModelRepository _modelRepo;
        private readonly ILocationRepository _locationRepo;
        private readonly IOidRepository _oidRepo;
        private readonly IUserProfileRepository _profileRepo;
        private readonly ISnmpService _snmpService;
        private readonly IAuditLogRepository _auditRepo;
        public BasePrinterManagementService(IBasePrinterRepository printRepo,
                                            IPrintModelRepository modelRepo,
                                            ILocationRepository locationRepo,
                                            IOidRepository oidRepo,
                                            IUserProfileRepository profileRepo,
                                            ISnmpService snmpService,
                                            IAuditLogRepository auditRepo)
        {
            _printRepo = printRepo;
            _modelRepo = modelRepo;
            _locationRepo = locationRepo;
            _oidRepo = oidRepo;
            _profileRepo = profileRepo;
            _snmpService = snmpService;
            _auditRepo = auditRepo;
        }

        public async Task<PrinterDTO> AddAsync(string title, string ipAddress, int locationId, int userProfileId)
        {
            ArgumentException.ThrowIfNullOrEmpty(ipAddress, nameof(ipAddress));
            var snmpResult = await _snmpService.GetModelAndSerialNumAsync(ipAddress);
            var userProfile = await _profileRepo.GetUserByIdAsync(userProfileId);

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
                        AuditLogs = new List<AuditLog>()
                    };
                    result = await _printRepo.AddAsync(newPrinter);
                    var newAuditLog = await _auditRepo.AddAsync(new AuditLog()
                    {
                        ActionType = "Add",
                        UserId = userProfileId,
                        Date = DateTime.UtcNow,
                        OldValue = "",
                        NewValue = $"Add new printer {result.SerialNumber}"
                    });

                    newPrinter.AuditLogs.Add(newAuditLog);
                    result = await _printRepo.UpdateAsync(newPrinter);

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
                    var newAuditLog = await _auditRepo.AddAsync(new AuditLog()
                    {
                        ActionType = "Add",
                        UserId = userProfileId,
                        Date = DateTime.UtcNow,
                        OldValue = "",
                        NewValue = $"Add user for printer {printer.SerialNumber}"
                    });
                    printer.UserProfiles.Add(userProfile);
                    printer.AuditLogs.Add(newAuditLog);
                    result = await _printRepo.UpdateAsync(printer);
                }
                catch (Exception ex)
                {
                    //TODO Написать обработчик ошибок
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }

            return new PrinterDTO()
            {
                Id = result.Id,
                IpAddress = result.IpAddress,
                LocationId = result.LocationId,
                ModelId = result.PrintModelId,
                Title = result.Title
            };
        }

        public async Task<bool> DeleteAsync(int id, int userProfileId)
        {
            try
            {
                var printer = await _printRepo.GetByIdAsync(id);
                var newAuditLog = new AuditLog()
                {
                    ActionType = "Delete",
                    UserId = userProfileId,
                    Date = DateTime.UtcNow,
                    OldValue = $"{printer.Id} {printer.Title} {printer.SerialNumber}",
                    NewValue = "Deleted"
                };
                var result = await _printRepo.DeleteAsync(printer);
                if (result)
                {
                    await _auditRepo.AddAsync(newAuditLog);
                }
                return result;
            }
            catch (Exception ex)
            {
                //TODO Написать обработчик ошибок
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<PrinterDTO> GetByIdAsync(int id)
        {
            var printer = await _printRepo.GetByIdAsync(id);
            var model = await _modelRepo.GetByIdAsync(printer.PrintModelId);
            var location = await _locationRepo.GetByIdAsync(printer.LocationId);
            var printerDTO = new PrinterDTO()
            {
                Id = id,
                Title = printer.Title,
                IpAddress = printer.IpAddress,
                LocationId = printer.LocationId,
                ModelId = printer.PrintModelId,
                Model = model.Title,
                Location = location.Title
            };
            return printerDTO;
        }

        public async Task<PrinterDetailDTO> GetDetailByIdAsync(int id)
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

        public async Task<PrinterDTO> UpdateAsync(PrinterDTO printer, int userProfileId)
        {
            ArgumentNullException.ThrowIfNull(printer);

            var editPrinter = await _printRepo.GetByIdAsync(printer.Id);
            editPrinter.Title = printer.Title;
            editPrinter.IpAddress = printer.IpAddress;
            editPrinter.PrintModelId = printer.ModelId;
            editPrinter.LocationId = printer.LocationId;
            editPrinter.AuditLogs.Add(new AuditLog
            {
                ActionType = "Update",
                UserId = userProfileId,
                Date = DateTime.UtcNow,
                OldValue = $"{editPrinter.Title}, {editPrinter.IpAddress}, {editPrinter.PrintModelId}, {editPrinter.LocationId}",
                NewValue = $"{printer.Title}, {printer.IpAddress}, {printer.ModelId}, {printer.LocationId}"
            });
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
