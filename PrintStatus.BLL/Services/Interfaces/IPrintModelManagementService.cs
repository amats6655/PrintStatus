namespace PrintStatus.BLL.Services.Interfaces;

using Helpers;

public interface IPrintModelManagementService
{
	Task<IServiceResponse<PrintModelDTO>> AddAsync(string modelTitle);
	Task<IServiceResponse<PrintModelDTO>> UpdateAsync(PrintModelDTO printModel, string identityUserId);
	Task<IServiceResponse<bool>> DeleteAsync(int id);
	Task<IServiceResponse<PrintModelDTO>> GetByIdAsync(int id);
	Task<IServiceResponse<IEnumerable<PrintModelDTO>>> GetAllAsync();
}

