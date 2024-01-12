using PrintStatus.DOM.Models;

namespace PrintStatus.BLL.Interfaces
{
	public interface ILocationManagementService
	{
		Task<LocationDTO> Add (LocationDTO location);
		Task<LocationDTO> Update(LocationDTO location);
		Task<bool> Delete (int id);
		Task<LocationDTO> GetById(int id);
		Task<IEnumerable<Location>> GetAll();
	}
}
