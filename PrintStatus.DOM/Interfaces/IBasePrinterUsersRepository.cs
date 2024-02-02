using PrintStatus.DOM.Models;

namespace PrintStatus.DOM.Interfaces
{
	public interface IBasePrinterUsersRepository
	{
		Task<IRepositoryResult<BasePrinterUser>> AddPrinterForUserAsync(string userId, int printerId);
		Task<IRepositoryResult<bool>> DeletePrinterFromUserAsync(string userId, int printerId);
		Task<IRepositoryResult<bool>> IsUserHasPrinterAsync(string userId, int printerId);

	}
}
