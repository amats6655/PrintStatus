using PrintStatus.DAL.Responses;
using PrintStatus.DOM.Models;

namespace PrintStatus.DAL.Repositories.Interfaces;

public interface IPrintOidRepository
{
	Task<IRepositoryResponse<PrintOid>> InsertAsync(PrintOid? oid);
	Task<IRepositoryResponse<PrintOid>> UpdateAsync(PrintOid oid);
	Task<IRepositoryResponse<PrintOid>> GetByIdAsync(int id);
	Task<IRepositoryResponse<PrintOid>> GetByValueAsync(string value);
	Task<IRepositoryResponse<List<PrintOid>>> GetAllAsync();
	Task<IRepositoryResponse<bool>> DeleteByIdAsync(int id);
}