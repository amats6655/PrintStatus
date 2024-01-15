using PrintStatus.DOM.Models;
namespace PrintStatus.DOM.Interfaces
{
	public interface IPrintModelRepository
	{
		Task<IRepositoryResult<PrintModel>> GetByIdAsync(int id);
		Task<IRepositoryResult<PrintModel>> AddAsync(PrintModel model);
		Task<IRepositoryResult<bool>> DeleteAsync(PrintModel model);
		Task<IRepositoryResult<PrintModel>> UpdateAsync(PrintModel model);
		Task<IRepositoryResult<IEnumerable<PrintModel>>> GetAllAsync();
		Task<IRepositoryResult<PrintModel>> GetByModelNameAsync(string modelName);
	}
}
