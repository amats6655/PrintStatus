using PrintStatus.DAL.Responses;
using PrintStatus.DOM.Models;

namespace PrintStatus.DAL.Repositories.Interfaces;

public interface IPrintModelRepository
{
	Task<IRepositoryResponse<PrintModel>> InsertAsync(PrintModel model);
	Task<IRepositoryResponse<bool>> DeleteByIdAsync(int id);
	Task<IRepositoryResponse<PrintModel>> UpdateAsync(PrintModel model);
	Task<IRepositoryResponse<List<PrintModel>>> GetAllAsync();
	Task<IRepositoryResponse<PrintModel>> GetByIdAsync(int id);
	Task<IRepositoryResponse<PrintModel>> GetByModelNameAsync(string modelName);
}