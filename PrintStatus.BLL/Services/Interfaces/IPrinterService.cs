using PrintStatus.BLL.DTO;

namespace PrintStatus.BLL.Services.Interfaces;

using Helpers;
using DOM.Models;

public interface IPrinterService
{
	Task<IServiceResponse<bool>> InsertAsync(NewPrinterDTO model, int userId);
	Task<IServiceResponse<List<Printer>>> GetAllAsync();
	Task<IServiceResponse<List<Printer>>> GetAllByLocationAsync(int locationId, int userId);
	Task<IServiceResponse<PrinterDetailDTO>> GetDetailByIdAsync(int id, int userId);
	Task<IServiceResponse<List<Printer>>> GetAllByModelAsync(int modelId, int userId);
	Task<IServiceResponse<List<Printer>>> GetAllByUserAsync(int userId);
	Task<IServiceResponse<Printer>> GetByIdAsync(int id);
	Task<IServiceResponse<bool>> DeleteAsync(int id, int userId);
	Task<IServiceResponse<Printer>> UpdateAsync(Printer model, int userId);
}