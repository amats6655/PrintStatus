using PrintStatus.DOM.Models;

namespace PrintStatus.DOM.Interfaces
{
	public interface IHistoryRepository
	{
		Task<IRepositoryResult<History>> GetByIdAsync(int id);
		Task<IRepositoryResult<History>> AddAsync(History history);
		Task<IRepositoryResult<IEnumerable<History>>> GetPrinterHistoriesAsync(int printerId);
		Task<IRepositoryResult<IEnumerable<History>>> GetPrinterOidHistoriesAsync(int printerId, int oidId);
		Task<IRepositoryResult<IEnumerable<History>>> GetAllAsync();
	}
}
