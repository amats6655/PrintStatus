using PrintStatus.BLL.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.BLL.Services
{
    public class HistoryManagementService : IHistoryManagementService
    {
        public Task<bool> AddHistory(int pritnerId, int OidId, string Value)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<History>> GetAllByPrinterId(int PrinterId)
        {
            throw new NotImplementedException();
        }

        public Task<History> GetById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
