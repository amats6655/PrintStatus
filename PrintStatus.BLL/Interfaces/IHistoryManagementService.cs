using PrintStatus.DOM.Models;

namespace PrintStatus.BLL.Interfaces
{
	public interface IHistoryManagementService
	{
		Task<IServiceResult<bool>> AddHistory(History history);
		Task<IServiceResult<History>> GetById(int id);
		Task<IServiceResult<IEnumerable<History>>> GetAllByPrinterId(int printerId);
		Task<IServiceResult<IEnumerable<History>>> GetAllHistories();
	}
}
