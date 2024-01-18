namespace PrintStatus.BLL.Interfaces
{
	public interface IPrintModelManagementService
	{
		Task<IServiceResult<PrintModelDTO>> AddAsync(string modelTitle);
		Task<IServiceResult<PrintModelDTO>> UpdateAsync(PrintModelDTO printModel, string identityUserId);
		Task<IServiceResult<bool>> DeleteAsync(int id);
		Task<IServiceResult<PrintModelDTO>> GetByIdAsync(int id);
		Task<IServiceResult<IEnumerable<PrintModelDTO>>> GetAllAsync();
	}
}
