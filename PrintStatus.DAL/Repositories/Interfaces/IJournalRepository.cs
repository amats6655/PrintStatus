using PrintStatus.DAL.Responses;
using PrintStatus.DOM.Models;

namespace PrintStatus.DAL.Repositories.Interfaces;

public interface IJournalRepository
{
	Task<IRepositoryResponse<Journal>> InsertAsync(Journal journal);
	Task<IRepositoryResponse<Journal>> UpdateAsync(Journal item);
	Task<IRepositoryResponse<Journal>> GetByIdAsync(int id);
	Task<IRepositoryResponse<List<Journal>>> GetAllAsync();
	Task<IRepositoryResponse<List<Journal>>> GetAllByPrinterIdAsync(int printerId);
	Task<IRepositoryResponse<List<Journal>>> GetAllByConsumableIdAsync(int printerId, int consumableId);
	Task<IRepositoryResponse<bool>> DeleteByIdAsync(int id);
}