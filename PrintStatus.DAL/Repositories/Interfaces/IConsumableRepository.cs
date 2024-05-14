using PrintStatus.DAL.Responses;
using PrintStatus.DOM.Models;

namespace PrintStatus.DAL.Repositories.Interfaces;

public interface IConsumableRepository
{
	Task<IRepositoryResponse<Consumable>> InsertAsync(Consumable? consumable);
	Task<IRepositoryResponse<Consumable>> UpdateAsync(Consumable consumable);
	Task<IRepositoryResponse<Consumable>> GetByIdAsync(int id);
	Task<IRepositoryResponse<List<Consumable>>> GetAllAsync();
	Task<IRepositoryResponse<bool>> DeleteByIdAsync(int id);
	Task<IRepositoryResponse<Consumable>> GetByNameAsync(string name);
	
	Task<IRepositoryResponse<List<Consumable>>> GetAllByModelIdAsync(int modelId);
}