using PrintStatus.DOM.Models;

namespace PrintStatus.DOM.Interfaces
{
	public interface IBasePrinterRepository
	{
		Task<BasePrinter> GetByIdAsync(int id);
		Task<IEnumerable<BasePrinter>> GetAllAsync();
		Task<BasePrinter> AddAsync(BasePrinter printer);
		Task<BasePrinter> GetIdBySerialNumberAsync(string serialNumber);
		Task<bool> DeleteAsync(BasePrinter printer);
		Task<BasePrinter> UpdateAsync(BasePrinter printer);
		Task<IEnumerable<BasePrinter>> GetAllByModelAsync(int modelId);
		Task<IEnumerable<BasePrinter>> GetAllByUserAsync(int userId);
		Task<IEnumerable<BasePrinter>> GetAllByLocationAsync(int locationId);
	}
}
