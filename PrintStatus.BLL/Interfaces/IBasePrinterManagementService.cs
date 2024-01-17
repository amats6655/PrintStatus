using PrintStatus.BLL.DTO;

namespace PrintStatus.BLL.Interfaces
{
	public interface IBasePrinterManagementService
	{
		Task<IServiceResult<PrinterDTO>> GetByIdAsync(int id);
		Task<IServiceResult<PrinterDetailDTO>> GetDetailByIdAsync(int id, string identityUserId);
		Task<IServiceResult<PrinterDTO>> AddAsync(NewPrinterDTO printer);
		Task<IServiceResult<PrinterDTO>> UpdateAsync(PrinterDTO printer, string identityUserId);
		Task<IServiceResult<bool>> DeleteAsync(int id, string identityUserId);
		Task<IServiceResult<IEnumerable<PrinterDTO>>> GetAllAsync();
		Task<IServiceResult<IEnumerable<PrinterDTO>>> GetAllByModelAsync(int modelId, string identityUserId);
		Task<IServiceResult<IEnumerable<PrinterDTO>>> GetAllByUserAsync(string identityUserId);
		Task<IServiceResult<IEnumerable<PrinterDTO>>> GetAllByLocationAsync(int locationId, string identityUserId);
	}
}
