using PrintStatus.DOM.Models;

namespace PrintStatus.DOM.Interfaces
{
	public interface IBasePrinterRepository
	{
		Task<IRepositoryResult<BasePrinter>> GetByIdAsync(int id);
		Task<IRepositoryResult<IEnumerable<BasePrinter>>> GetAllAsync();
		Task<IRepositoryResult<BasePrinter>> AddAsync(BasePrinter printer);
		Task<IRepositoryResult<BasePrinter>> GetBySerialNumberAsync(string serialNumber);
		Task<IRepositoryResult<bool>> DeleteAsync(int id);
		Task<IRepositoryResult<BasePrinter>> UpdateAsync(BasePrinter printer);
		Task<IRepositoryResult<IEnumerable<BasePrinter>>> GetAllByModelAsync(int modelId, string identityUserId);
		Task<IRepositoryResult<IEnumerable<BasePrinter>>> GetAllByUserAsync(string identityUserId);
		Task<IRepositoryResult<IEnumerable<BasePrinter>>> GetAllByLocationAsync(int locationId, string identityUserId);
	}
}
