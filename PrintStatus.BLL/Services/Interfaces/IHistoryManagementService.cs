namespace PrintStatus.BLL.Services.Interfaces;

using PrintStatus.DOM.Models;
using Helpers;

public interface IHistoryManagementService
{
	Task<IServiceResponse<bool>> AddHistoryAsync(Journal journal);
	Task<IServiceResponse<Journal>> GetByIdAsync(int id);
	Task<IServiceResponse<IEnumerable<Journal>>> GetAllByPrinterIdAsync(int printerId);
	Task<IServiceResponse<IEnumerable<Journal>>> GetAllHistoriesAsync();
}

