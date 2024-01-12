using PrintStatus.DOM.Models;

namespace PrintStatus.BLL.Interfaces
{
	public interface IHistoryManagementService
	{
		Task<bool> AddHistory(int pritnerId, int OidId, string Value);
		Task<History> GetById(int id);
		Task<IEnumerable<History>> GetAllByPrinterId (int PrinterId);
	}
}
