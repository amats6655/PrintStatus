using PrintStatus.DOM.Models;

namespace PrintStatus.DOM.Interfaces
{
	public interface ILocationRepository
	{
		Task<Location> GetByIdAsync(int id);
		Task<IEnumerable<Location>> GetAllAsync();
		Task<Location> AddAsync(Location location);
		Task<bool> DeleteAsync(Location location);
		Task<Location> UpdateAsync(Location location);
		Task<Location> GetByTitle(string Title);
	}
}
