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
		public async Task<IRepositoryResult<BasePrinter>> AddAsync(BasePrinter printer)
		{
			if (printer == null) return new RepositoryResult<BasePrinter>().HandleException(new ArgumentNullException(nameof(printer)));
			var printerExist = await _context.BasePrinters.AnyAsync(p => p.SerialNumber == printer.SerialNumber);
			if (printerExist) return RepositoryResult<BasePrinter>.Failure(new List<string> {""},  $"Принтер {printer.SerialNumber} уже существует" );
			try
			{
				await _context.BasePrinters.AddAsync(printer);
				await _context.SaveChangesAsync();
				return RepositoryResult<BasePrinter>.Success(printer, $"{printer.Title} создан");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<BasePrinter>().HandleException(ex);
			}
		}

		public async Task<IRepositoryResult<bool>> DeleteAsync(BasePrinter printer)
		{
			if (printer == null) return new RepositoryResult<bool>().HandleException(new ArgumentNullException(nameof(printer)));
			var printerExist = await _context.BasePrinters.FindAsync(printer.Id);
			if (printerExist == null) return RepositoryResult<bool>.Failure(new List<string> {""}, $"Принтер {printer.Id} не найден" );
			try
			{
				_context.BasePrinters.Remove(printer);
				await _context.SaveChangesAsync();
				return RepositoryResult<bool>.Success(true, "Принтер удален");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<bool>().HandleException(ex);
			}
		}

		public async Task<IRepositoryResult<IEnumerable<BasePrinter>>> GetAllAsync()
		{
			try
			{
				var printers = await _context.BasePrinters.AsNoTracking().ToListAsync();
				return RepositoryResult<IEnumerable<BasePrinter>>.Success(printers, $"Получено {printers.Count} записей");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<IEnumerable<BasePrinter>>().HandleException(ex);
			}
		}

		public async Task<IRepositoryResult<IEnumerable<BasePrinter>>> GetAllByLocationAsync(int locationId, string identityUserId)
		{
			if (locationId <= 0 || string.IsNullOrEmpty(identityUserId)) return new RepositoryResult<IEnumerable<BasePrinter>>()
					.HandleException(new ArgumentNullException(nameof(locationId), nameof(identityUserId)));
			try
			{
				var printers = await _context.BasePrinters
								.AsNoTracking()
								.Include(p => p.PrintModel)
								.Include(p => p.Location)
								.Where(p => p.LocationId == locationId && p.UserProfiles
									.Any(u => u.IdentityId == identityUserId))
								.ToListAsync();
				return RepositoryResult<IEnumerable<BasePrinter>>.Success(printers, $"Получено {printers.Count} записей");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<IEnumerable<BasePrinter>>().HandleException(ex);
			}
		}

		public async Task<IRepositoryResult<IEnumerable<BasePrinter>>> GetAllByModelAsync(int modelId, string identityUserId)
		{
			if (modelId <= 0 || string.IsNullOrEmpty(identityUserId)) return new RepositoryResult<IEnumerable<BasePrinter>>()
					.HandleException(new ArgumentNullException(nameof(modelId), nameof(identityUserId)));
			try
			{
				var printers = await _context.BasePrinters
								.AsNoTracking()
								.Include(p => p.PrintModel)
								.Include(p => p.Location)
								.Where(p => p.PrintModelId == modelId && p.UserProfiles
									.Any(u => u.IdentityId == identityUserId))
								.ToListAsync();
				return RepositoryResult<IEnumerable<BasePrinter>>.Success(printers, $"Получено {printers.Count} записей");

			}
			catch (Exception ex)
			{
				return new RepositoryResult<IEnumerable<BasePrinter>>().HandleException(ex);
			}
		}

		public async Task<IRepositoryResult<IEnumerable<BasePrinter>>> GetAllByUserAsync(string identityUserId)
		{
			if (string.IsNullOrEmpty(identityUserId)) return new RepositoryResult<IEnumerable<BasePrinter>>()
					.HandleException(new ArgumentNullException(nameof(identityUserId)));
			try
			{
				var printers = await _context.BasePrinters
								.AsNoTracking()
								.Include(p => p.PrintModel)
								.Include(p => p.Location)
								.Where(p => p.UserProfiles
									.Any(u => u.IdentityId == identityUserId))
								.ToListAsync();
				return RepositoryResult<IEnumerable<BasePrinter>>.Success(printers, $"Получено {printers.Count} записей");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<IEnumerable<BasePrinter>>().HandleException(ex);
			}
		}

		public async Task<IRepositoryResult<BasePrinter>> GetByIdAsync(int id)
		{
			if (id <= 0) return new RepositoryResult<BasePrinter>().HandleException(new ArgumentNullException(nameof(id)));
			try
			{
				var result = await _context.BasePrinters.FindAsync(id);
				if (result == null) return RepositoryResult<BasePrinter>.Failure(new List<string>(), $"Не удалось найти принтер с id = {id}" );
				return RepositoryResult<BasePrinter>.Success(result, "Принтер получен");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<BasePrinter>().HandleException(ex);
			}
		}

		public async Task<IRepositoryResult<BasePrinter>> GetBySerialNumberAsync(string serialNumber)
		{
			if (string.IsNullOrEmpty(serialNumber)) return new RepositoryResult<BasePrinter>().HandleException(new ArgumentNullException(nameof(serialNumber)));
			ArgumentException.ThrowIfNullOrEmpty(nameof(serialNumber));
			try
			{
				var result = await _context.BasePrinters
								.Include(p => p.PrintModel)
								.Include(p => p.Location)
								.Where(p => p.SerialNumber
									.Equals(serialNumber))
								.FirstOrDefaultAsync();
				if (result == null) return RepositoryResult<BasePrinter>.Failure(new List<string>(), $"Не удалось найти принтер с серийным номером = {serialNumber}" );
				return RepositoryResult<BasePrinter>.Success(result, "Принтер получен");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<BasePrinter>().HandleException(ex);
			}
		}

		public async Task<IRepositoryResult<BasePrinter>> UpdateAsync(BasePrinter printer)
		{
			if (printer == null) return new RepositoryResult<BasePrinter>().HandleException(new ArgumentNullException(nameof(printer)));
			var printerExist = await _context.BasePrinters.FindAsync(printer.Id);
			if (printerExist == null) return RepositoryResult<BasePrinter>.Failure(new List<string> {""}, "Не найден изменяемый объект" );
			try
			{
				printerExist.Title = printer.Title;
				printerExist.IpAddress = printer.IpAddress;
				printerExist.SerialNumber = printer.SerialNumber;
				printerExist.PrintModelId = printer.PrintModelId;
				_context.BasePrinters.Update(printer);
				await _context.SaveChangesAsync();
				return RepositoryResult<BasePrinter>.Success(printerExist, "Принтер обновлен");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<BasePrinter>().HandleException(ex);
			}
		}
	}
}
