using PrintStatus.DOM.Models;

namespace PrintStatus.DOM.Interfaces
{
    public interface IHistoryRepository
    {
        Task<History> GetByIdAsync(int id);
        Task<History> AddAsync(History history);
        Task<IEnumerable<History>> GetPrinterHistoriesAsync(int printerId);
        Task<IEnumerable<History>> GetPrinterOidHistoriesAsync(int printerId, int oidId);
        Task<IEnumerable<History>> GetAllAsync();
    }
}
