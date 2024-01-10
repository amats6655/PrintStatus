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
		public async Task<History> AddAsync(History history)
		{
			ArgumentNullException.ThrowIfNull(history);
			try
			{
				_context.Histories.Add(history);
				await _context.SaveChangesAsync();
				return history;
			}
			catch (Exception ex)
			{
				//TODO Добавить обработчик ошибок
				Console.WriteLine(ex.Message);
				return null;
			}
		}

		public async Task<IEnumerable<History>> GetAllAsync()
		{
			try
			{
				return await _context.Histories.ToListAsync();
			}
			catch (Exception ex)
			{
				//TODO Добавить обработчик ошибок
				Console.WriteLine(ex.Message);
				return Enumerable.Empty<History>();
			}
		}

		public async Task<History> GetByIdAsync(int id)
		{
			try
			{
				return await _context.Histories.FindAsync(id);
			}
			catch (Exception ex)
			{
				//TODO Добавить обработчик ошибок
				Console.WriteLine(ex.Message);
				return null;
			}
		}

		public async Task<IEnumerable<History>> GetPrinterHistoriesAsync(int printerId)
		{
			try
			{
				return await _context.Histories
								.Where(h => h.BasePrinterId == printerId)
								.ToListAsync();
			}
			catch (Exception ex)
			{
				//TODO Добавить обработчик ошибок
				Console.WriteLine(ex.Message);
				return Enumerable.Empty<History>();
			}
		}

		public async Task<IEnumerable<History>> GetPrinterOidHistoriesAsync(int printerId, int oidId)
		{
			try
			{
				return await _context.Histories
								.Where(h => h.BasePrinterId == printerId
										&& h.OidId == oidId)
								.ToListAsync();
			}
			catch (Exception ex)
			{
				//TODO Добавить обработчик ошибок
				Console.WriteLine(ex.Message);
				return Enumerable.Empty<History>();
			}
		}
	}
}
