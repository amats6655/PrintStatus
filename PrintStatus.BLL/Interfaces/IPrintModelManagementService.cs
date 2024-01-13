namespace PrintStatus.BLL.Interfaces
{
	public interface IPrintModelManagementService
	{
		Task<PrintModelDTO> AddAsync(string modelTitle);
		Task<PrintModelDTO> UpdateAsync(PrintModelDTO printModel);
		Task<bool> DeleteAsync(int id);
		Task<PrintModelDTO> GetByIdAsync(int id);
		Task<IEnumerable<PrintModelDTO>> GetAllAsync();
	}
}
