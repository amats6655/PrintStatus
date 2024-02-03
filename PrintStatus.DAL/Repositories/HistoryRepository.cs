using Microsoft.EntityFrameworkCore;
using PrintStatus.DAL.Connection;
using PrintStatus.DOM.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.DAL.Repositories
{
	public class HistoryRepository : IHistoryRepository
	{
		private readonly ApplicationDbContext _context;
		public HistoryRepository(ApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<IRepositoryResult<History>> AddAsync(History history)
		{
			if (history == null) return new RepositoryResult<History>().HandleException(new ArgumentNullException(nameof(history)));
			try
			{
				await _context.Histories.AddAsync(history);
				await _context.SaveChangesAsync();
				return RepositoryResult<History>.Success(history, "Запись создана");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<History>().HandleException(ex);
			}
		}

		public async Task<IRepositoryResult<IEnumerable<History>>> GetAllAsync()
		{
			try
			{
				var histories = await _context.Histories.AsNoTracking().ToListAsync();
				return RepositoryResult<IEnumerable<History>>.Success(histories, $"Получено {histories.Count} записей");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<IEnumerable<History>>().HandleException(ex);
			}
		}

		public async Task<IRepositoryResult<History>> GetByIdAsync(int id)
		{
			if (id <= 0) return new RepositoryResult<History>().HandleException(new ArgumentNullException(nameof(id)));
			try
			{
				var result = await _context.Histories.FindAsync(id);
				if (result == null) return RepositoryResult<History>.Failure(new List<string> (), $"Не удалось найти историю с id = {id}");
				return RepositoryResult<History>.Success(result, "История получена");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<History>().HandleException(ex);
			}
		}

		public async Task<IRepositoryResult<IEnumerable<History>>> GetPrinterHistoriesAsync(int printerId)
		{
			if (printerId <= 0) return new RepositoryResult<IEnumerable<History>>().HandleException(new ArgumentNullException(nameof(printerId)));
			try
			{
				var histories = await _context.Histories
								.AsNoTracking()
								.Where(h => h.BasePrinterId == printerId)
								.ToListAsync();
				return RepositoryResult<IEnumerable<History>>.Success(histories, $"Получено {histories.Count} записей");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<IEnumerable<History>>().HandleException(ex);
			}
		}

		public async Task<IRepositoryResult<IEnumerable<History>>> GetPrinterOidHistoriesAsync(int printerId, int oidId)
		{
			if (printerId <= 0 || oidId <= 0) return new RepositoryResult<IEnumerable<History>>().HandleException(new ArgumentNullException(nameof(printerId), nameof(oidId)));
			try
			{
				var histories = await _context.Histories
								.AsNoTracking()
								.Where(h => h.BasePrinterId == printerId
										&& h.PrintOidId == oidId)
								.ToListAsync();
				return RepositoryResult<IEnumerable<History>>.Success(histories, $"Получено {histories.Count} записей");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<IEnumerable<History>>().HandleException(ex);
			}
		}
	}
}
