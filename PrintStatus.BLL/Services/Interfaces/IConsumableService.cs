using PrintStatus.BLL.DTO;
using PrintStatus.BLL.Helpers;
using PrintStatus.DOM.Models;

namespace PrintStatus.BLL.Services.Interfaces;

public interface IConsumableService
{
	Task<IServiceResponse<bool>>InsertAsync(NewConsumableDTO model);
	Task<IServiceResponse<List<Consumable>>> GetAllAsync();
	Task<IServiceResponse<Consumable>> GetByIdAsync(int id);
	Task<IServiceResponse<bool>> DeleteAsync(int id);
	Task<IServiceResponse<Consumable>> UpdateAsync(Consumable model);
	
	Task<IServiceResponse<List<Consumable>>> GetAllByModelAsync(int modelId);
}