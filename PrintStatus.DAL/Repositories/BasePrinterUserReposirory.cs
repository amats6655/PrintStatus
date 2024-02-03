using PrintStatus.DAL.Connection;
using PrintStatus.DOM.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.DAL.Repositories
{
	public class BasePrinterUserReposirory : IBasePrinterUsersRepository
	{
		private readonly ApplicationDbContext _context;
		public BasePrinterUserReposirory(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IRepositoryResult<BasePrinterUser>> AddPrinterForUserAsync(string userId, int printerId)
		{
			if (string.IsNullOrEmpty(userId) || printerId <= 0)
				return new RepositoryResult<BasePrinterUser>().HandleException(new ArgumentNullException());

			var printerExist = await _context.BasePrinters.FindAsync(printerId);

			if (printerExist == null)
				return RepositoryResult<BasePrinterUser>.Failure(new List<string>(), "Принтер не найден");

			try
			{
				var basePrinterUser = new BasePrinterUser { UserId = userId, BasePrinterId = printerId };
				await _context.AddAsync(basePrinterUser);
				await _context.SaveChangesAsync();
				return RepositoryResult<BasePrinterUser>.Success(basePrinterUser, "Принтер добавлен пользователю");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<BasePrinterUser>().HandleException(ex);
			}
		}

		public async Task<IRepositoryResult<bool>> DeletePrinterFromUserAsync(string userId, int printerId)
		{
			if (string.IsNullOrEmpty(userId) || printerId <= 0)
				return new RepositoryResult<bool>().HandleException(new ArgumentNullException());
			var printerUserExist = await _context.BasePrinterUsers.FindAsync(userId, printerId);
			if (printerUserExist == null)
				return RepositoryResult<bool>.Failure(new List<string>(), "Связь не найдена");
			try
			{
				_context.BasePrinterUsers.Remove(printerUserExist);
				await _context.SaveChangesAsync();
				return RepositoryResult<bool>.Success(true, "Связь удалена");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<bool>().HandleException(ex);
			}
		}

		public async Task<IRepositoryResult<bool>> IsUserHasPrinterAsync(string userId, int printerId)
		{
			if (string.IsNullOrEmpty(userId) || printerId <= 0)
				return new RepositoryResult<bool>().HandleException(new ArgumentNullException());

			var printerUserExist = await _context.BasePrinterUsers.FindAsync(userId, printerId);
			if (printerUserExist == null)
				return RepositoryResult<bool>.Success(false, "Связь не найдена");
			return RepositoryResult<bool>.Success(true, "Связь найдена");
		}
	}
}
