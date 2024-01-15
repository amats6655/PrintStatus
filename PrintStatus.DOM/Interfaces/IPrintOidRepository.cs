using PrintStatus.DOM.Models;

namespace PrintStatus.DOM.Interfaces
{
	public interface IPrintOidRepository
	{
		Task<IRepositoryResult<PrintOid>> GetByIdAsync(int id);
		Task<IRepositoryResult<PrintOid>> AddAsync(PrintOid oid);
		Task<IRepositoryResult<bool>> DeleteAsync(int id);
		Task<IRepositoryResult<PrintOid>> UpdateAsync(PrintOid oid);
		Task<IRepositoryResult<IEnumerable<PrintOid>>> GetAllAsync();
		Task<IRepositoryResult<IEnumerable<PrintOid>>> GetAllByModelIdAsync(int modelId);
	}
}
