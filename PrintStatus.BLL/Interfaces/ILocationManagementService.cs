namespace PrintStatus.BLL.Interfaces
{
	public interface ILocationManagementService
	{
		Task<IServiceResult<LocationDTO>> AddAsync(LocationDTO location);
		Task<IServiceResult<LocationDTO>> UpdateAsync(LocationDTO location);
		Task<IServiceResult<bool>> DeleteAsync(int id);
		Task<IServiceResult<LocationDTO>> GetByIdAsync(int id);
		Task<IServiceResult<IEnumerable<LocationDTO>>> GetAllAsync();
	}
}
