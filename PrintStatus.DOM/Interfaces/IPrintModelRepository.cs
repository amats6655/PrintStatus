using PrintStatus.DOM.Models;
namespace PrintStatus.DOM.Interfaces
{
	public interface IPrintModelRepository
	{
		Task<PrintModel> GetByIdAsync(int id);
		Task<PrintModel> AddAsync(PrintModel model);
		Task<bool> DeleteAsync(PrintModel model);
		Task<PrintModel> UpdateAsync(PrintModel model);
		Task<IEnumerable<PrintModel>> GetAllAsync();
		Task<PrintModel> GetIdByModelNameAsync(string modelName);
	}
}
