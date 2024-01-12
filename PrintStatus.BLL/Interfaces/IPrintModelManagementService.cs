using PrintStatus.DOM.Models;

namespace PrintStatus.BLL.Interfaces
{
	public interface IPrintModelManagementService
	{
		Task<PrintModelDTO> Add(PrintModelDTO printModel);
		Task<PrintModelDTO> Update (PrintModelDTO printModel);
		Task<bool> Delete (int id);
		Task<PrintModelDTO> GetById(int id);
		Task<IEnumerable<PrintModelDTO>> GetAll();
	}
}
