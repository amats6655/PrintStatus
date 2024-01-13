using PrintStatus.DOM.Models;

namespace PrintStatus.DOM.Interfaces
{
    public interface IPrintOidRepository
    {
        Task<PrintOid> GetByIdAsync(int id);
        Task<PrintOid> AddAsync(PrintOid oid);
        Task<bool> DeleteAsync(PrintOid oid);
        Task<PrintOid> UpdateAsync(PrintOid oid);
        Task<IEnumerable<PrintOid>> GetAllAsync();
        Task<IEnumerable<PrintOid>> GetAllByModelIdAsync(int modelId);
    }
}
