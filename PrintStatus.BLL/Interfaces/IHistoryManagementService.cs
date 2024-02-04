using PrintStatus.DOM.Models;

namespace PrintStatus.BLL.Interfaces
{
	public interface IHistoryManagementService
	{
		Task<IServiceResult<bool>> AddHistoryAsync(History history);
		Task<IServiceResult<History>> GetByIdAsync(int id);
		Task<IServiceResult<IEnumerable<History>>> GetAllByPrinterIdAsync(int printerId);
		Task<IServiceResult<IEnumerable<History>>> GetAllHistoriesAsync();
	}
}
