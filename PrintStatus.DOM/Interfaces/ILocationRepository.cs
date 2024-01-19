using PrintStatus.DOM.Models;

namespace PrintStatus.DOM.Interfaces
{
	public interface ILocationRepository
	{
		Task<IRepositoryResult<Location>> GetByIdAsync(int id);
		Task<IRepositoryResult<IEnumerable<Location>>> GetAllAsync();
		Task<IRepositoryResult<Location>> AddAsync(Location location);
		Task<IRepositoryResult<bool>> DeleteAsync(int id);
		Task<IRepositoryResult<Location>> UpdateAsync(Location location);
		Task<IRepositoryResult<Location>> GetByTitleAsync(string Title);
	}
}
