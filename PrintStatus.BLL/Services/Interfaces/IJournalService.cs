namespace PrintStatus.BLL.Services.Interfaces;

using Helpers;
using DOM.Models;

public interface IJournalService
{
	Task<IServiceResponse<bool>> InsertAsync(Journal model);
	Task<IServiceResponse<List<Journal>>> GetAllAsync();
	Task<IServiceResponse<Journal>> GetByIdAsync(int id);
	Task<IServiceResponse<bool>> DeleteAsync(int id);
	Task<IServiceResponse<Journal>> UpdateAsync(Journal model);
	Task<IServiceResponse<List<Journal>>> GetAllByPrinterIdAsync(int printerId);
	Task<IServiceResponse<List<Journal>>> GetAllByOidIdAsync(int printerId, int modelId);
}