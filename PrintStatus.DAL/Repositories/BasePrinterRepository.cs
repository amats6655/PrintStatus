using Microsoft.EntityFrameworkCore;
using PrintStatus.DAL.Connection;
using PrintStatus.DOM.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.DAL.Repositories
{
	public class BasePrinterRepository : IBasePrinterRepository
	{
		private readonly ApplicationDbContext _context;
		public BasePrinterRepository(ApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<BasePrinter> AddAsync(BasePrinter printer)
		{
			ArgumentNullException.ThrowIfNull(printer);
			try
			{
				_context.BasePrinters.Add(printer);
				await _context.SaveChangesAsync();
				return printer;
			}
			catch (Exception ex)
			{
				//TODO Добавить обработчик ошибок
				Console.WriteLine(ex.Message);
				return null;
			}
		}

		public async Task<bool> DeleteAsync(BasePrinter printer)
		{
			ArgumentNullException.ThrowIfNull(printer);
			try
			{
				_context.BasePrinters.Remove(printer);
				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				//TODO Добавить обработчик ошибок
				Console.WriteLine(ex.Message);
				return false;
			}
		}

        public async Task<IEnumerable<BasePrinter>> GetAllAsync()
		{
			try
			{
				return await _context.BasePrinters.ToListAsync();
			}
			catch (Exception ex)
			{
				//TODO Добавить обработчик ошибок
				Console.WriteLine(ex.Message);
				return Enumerable.Empty<BasePrinter>();
			}
		}

		public async Task<IEnumerable<BasePrinter>> GetAllByLocationAsync(int locationId)
		{
			try
			{
				return await _context.BasePrinters
								.Where(p => p.LocationId == locationId)
								.ToListAsync();
			}
			catch (Exception ex)
			{
				//TODO Добавить обработчик ошибок
				Console.WriteLine(ex.Message);
				return Enumerable.Empty<BasePrinter>();
			}
		}

		public async Task<IEnumerable<BasePrinter>> GetAllByModelAsync(int modelId)
		{
			try
			{
				return await _context.BasePrinters
								.Where(p => p.PrintModelId == modelId)
								.ToListAsync();
			}
			catch (Exception ex)
			{
				//TODO Добавить обработчик ошибок
				Console.WriteLine(ex.Message);
				return Enumerable.Empty<BasePrinter>();
			}
		}

		public async Task<IEnumerable<BasePrinter>> GetAllByUserAsync(int userId)
		{
			try
			{
				return await _context.BasePrinters
								.Where(p => p.UserProfiles
								.Any(u => u.Id == userId))
								.ToListAsync();
			}
			catch (Exception ex)
			{
				//TODO Добавить обработчик ошибок
				Console.WriteLine(ex.Message);
				return Enumerable.Empty<BasePrinter>();
			}
		}

		public async Task<BasePrinter> GetByIdAsync(int id)
		{
			try
			{
				return await _context.BasePrinters.FindAsync(id);
			}
			catch (Exception ex)
			{
				//TODO Добавить обработчик ошибок
				Console.WriteLine(ex.Message);
				return null;
			}
		}

		public async Task<BasePrinter> GetIdBySerialNumberAsync(string serialNumber)
		{
			ArgumentException.ThrowIfNullOrEmpty(nameof(serialNumber));
			try
			{
				return await _context.BasePrinters
								.Where(p => p.SerialNumber
								.Equals(serialNumber))
								.FirstOrDefaultAsync();
			}
			catch (Exception ex)
			{
				//TODO добавить обработчик ошибок
				Console.WriteLine(ex.Message);
				return null;
			}
		}

		public async Task<BasePrinter> UpdateAsync(BasePrinter printer)
		{
			ArgumentNullException.ThrowIfNull(printer);
			try
			{
				_context.BasePrinters.Update(printer);
				await _context.SaveChangesAsync();
				return printer;
			}
			catch (Exception ex)
			{
				//TODO Добавить обработчик ошибок
				Console.WriteLine(ex.Message);
				return null;
			}
		}
	}
}
