using PrintStatus.DAL.Responses;
using PrintStatus.DOM.Models;

namespace PrintStatus.DAL.Repositories.Interfaces;

public interface ILocationRepository
{
	Task<IRepositoryResponse<Location>> InsertAsync(Location location);
	Task<IRepositoryResponse<bool>> DeleteByIdAsync(int id);
	Task<IRepositoryResponse<Location>> UpdateAsync(Location location);
	Task<IRepositoryResponse<List<Location>>> GetAllAsync();
	Task<IRepositoryResponse<Location>> GetByIdAsync(int id);
	Task<IRepositoryResponse<Location>> GetByNameAsync(string name);
}