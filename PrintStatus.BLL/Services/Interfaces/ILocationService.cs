namespace PrintStatus.BLL.Services.Interfaces;

using Helpers;
using DOM.Models;

public interface ILocationService
{
	Task<IServiceResponse<bool>> InsertAsync(LocationDTO model);
	Task<IServiceResponse<List<Location>>> GetAllAsync();
	Task<IServiceResponse<Location>> GetByIdAsync(int id);
	Task<IServiceResponse<bool>> DeleteAsync(int id);
	Task<IServiceResponse<Location>> UpdateAsync(Location model);
}