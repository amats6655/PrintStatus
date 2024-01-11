using PrintStatus.BLL.DTO;

namespace PrintStatus.BLL.Interfaces
{
	public interface IBasePrinterManagementService
	{
		Task<PrinterDTO> GetByIdAsync(int id, string identityUserId);
		Task<PrinterDetailDTO> GetDetailByIdAsync(int id, string identityUserId);
		Task<PrinterDTO> AddAsync(string title, string ipAddress, int locationId, string identityUserId);
		Task<PrinterDTO> UpdateAsync(PrinterDTO printer, string identityUserId);
		Task<bool> DeleteAsync(int id, string identityUserId);
		Task<IEnumerable<PrinterDTO>> GetAllAsync();
		Task<IEnumerable<PrinterDTO>> GetAllByModelAsync(int modelId, string identityUserId);
		Task<IEnumerable<PrinterDTO>> GetAllByUserAsync(string identityUserId);
		Task<IEnumerable<PrinterDTO>> GetAllByLocationAsync(int locationId, string identityUserId);
	}
}
