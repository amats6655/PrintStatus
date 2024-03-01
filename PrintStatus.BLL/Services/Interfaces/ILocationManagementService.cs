namespace PrintStatus.BLL.Services.Interfaces;

using Helpers;

public interface ILocationManagementService
{
	Task<IServiceResponse<LocationDTO>> AddAsync(LocationDTO location);
	Task<IServiceResponse<LocationDTO>> UpdateAsync(LocationDTO location, string identityUserId);
	Task<IServiceResponse<bool>> DeleteAsync(int id);
	Task<IServiceResponse<LocationDTO>> GetByIdAsync(int id);
	Task<IServiceResponse<IEnumerable<LocationDTO>>> GetAllAsync();
}

