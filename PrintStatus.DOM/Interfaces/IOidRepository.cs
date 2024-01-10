using PrintStatus.DOM.Models;

namespace PrintStatus.DOM.Interfaces
{
    public interface IOidRepository
    {
        Task<Oid> GetByIdAsync(int id);
        Task<Oid> AddAsync(Oid oid);
        Task<bool> DeleteAsync(Oid oid);
        Task<Oid> UpdateAsync(Oid oid);
        Task<IEnumerable<Oid>> GetAllAsync();
        Task<IEnumerable<Oid>> GetAllByModelIdAsync(int modelId);
    }
}
