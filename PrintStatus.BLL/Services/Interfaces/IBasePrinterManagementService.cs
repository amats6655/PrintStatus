namespace PrintStatus.BLL.Services.Interfaces;

using DTO;
using Helpers;

public interface IBasePrinterManagementService
{
	Task<IServiceResponse<PrinterDTO>> GetByIdAsync(int id);
	Task<IServiceResponse<PrinterDetailDTO>> GetDetailByIdAsync(int id, string identityUserId);
	Task<IServiceResponse<PrinterDTO>> AddAsync(NewPrinterDTO printer);
	Task<IServiceResponse<PrinterDTO>> UpdateAsync(PrinterDTO printer, string identityUserId);
	Task<IServiceResponse<bool>> DeleteAsync(int id, string identityUserId);
	Task<IServiceResponse<IEnumerable<PrinterDTO>>> GetAllAsync();
	Task<IServiceResponse<IEnumerable<PrinterDTO>>> GetAllByModelAsync(int modelId, string identityUserId);
	Task<IServiceResponse<IEnumerable<PrinterDTO>>> GetAllByUserAsync(string identityUserId);
	Task<IServiceResponse<IEnumerable<PrinterDTO>>> GetAllByLocationAsync(int locationId, string identityUserId);
}

