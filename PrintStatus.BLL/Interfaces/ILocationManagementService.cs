namespace PrintStatus.BLL.Interfaces
{
	public interface ILocationManagementService
	{
		Task<LocationDTO> AddAsync(LocationDTO location);
		Task<LocationDTO> UpdateAsync(LocationDTO location);
		Task<bool> DeleteAsync(int id);
		Task<LocationDTO> GetByIdAsync(int id);
		Task<IEnumerable<LocationDTO>> GetAllAsync();
	}
}
