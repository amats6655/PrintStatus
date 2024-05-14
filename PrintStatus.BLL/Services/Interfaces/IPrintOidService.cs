using PrintStatus.BLL.DTO;

namespace PrintStatus.BLL.Services.Interfaces;

using DOM.Models;
using Helpers;

public interface IPrintOidService
{
	Task<IServiceResponse<bool>> InsertAsync(NewPrintOidDTO model);
	Task<IServiceResponse<List<PrintOid>>> GetAllAsync();
	Task<IServiceResponse<PrintOid>> GetByIdAsync(int id);
	Task<IServiceResponse<bool>> DeleteAsync(int id);
	Task<IServiceResponse<PrintOid>> UpdateAsync(PrintOid model);
}