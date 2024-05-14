namespace PrintStatus.BLL.Services.Interfaces;

using Helpers;
using DOM.Models;

public interface IPrintModelService
{
	Task<IServiceResponse<PrintModel>> InsertAsync(string modelName, int CalcTypeId = 1);
	Task<IServiceResponse<List<PrintModel>>> GetAllAsync();
	Task<IServiceResponse<PrintModel>> GetByIdAsync(int id);
	Task<IServiceResponse<bool>> DeleteAsync(int id);
	Task<IServiceResponse<PrintModel>> UpdateAsync(PrintModel model);
}