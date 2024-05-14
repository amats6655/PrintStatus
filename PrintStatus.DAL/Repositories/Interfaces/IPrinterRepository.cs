using PrintStatus.DAL.Responses;
using PrintStatus.DOM.Models;

namespace PrintStatus.DAL.Repositories.Interfaces;

public interface IPrinterRepository
{
	Task<IRepositoryResponse<Printer>> InsertAsync(Printer printer);
	Task<IRepositoryResponse<bool>> DeleteByIdAsync(int id);
	Task<IRepositoryResponse<Printer>> UpdateAsync(Printer printer);
	Task<IRepositoryResponse<Printer>> GetByIdAsync(int id);
	Task<IRepositoryResponse<List<Printer>>> GetAllAsync();
	Task<IRepositoryResponse<List<Printer>>> GetAllByLocationAsync(int locationId);
	Task<IRepositoryResponse<List<Printer>>> GetAllByModelAsync(int printModelId);
	Task<IRepositoryResponse<List<Printer>>> GetAllByUserAsync(int userId);
	Task<IRepositoryResponse<Printer>> GetBySerialNumberAsync(string serialNumber);
}