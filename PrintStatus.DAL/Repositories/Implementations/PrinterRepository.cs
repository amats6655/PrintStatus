namespace PrintStatus.DAL.Repositories.Implementations;

using Data;
using DOM.Models;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Responses;

public class PrinterRepository(AppDbContext _context) : IGenericRepositoryInterface<Printer>
{
	public async Task<IRepositoryResponse<Printer>> InsertAsync(Printer printer)
	{
		if (printer == null) return new RepositoryResponse<Printer>().HandleException(new ArgumentNullException(nameof(printer)));
		var printerExist = await _context.Printers.AnyAsync(p => p.SerialNumber == printer.SerialNumber);
		if (printerExist) return RepositoryResponse<Printer>.Failure(new List<string> { "" }, $"Принтер {printer.SerialNumber} уже существует");
		try
		{
			await _context.Printers.AddAsync(printer);
			await _context.SaveChangesAsync();
			return RepositoryResponse<Printer>.Success(printer, $"{printer.Title} создан");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<Printer>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<bool>> DeleteByIdAsync(int id)
	{
		if (id <= 0) return new RepositoryResponse<bool>().HandleException(new ArgumentNullException(nameof(id)));
		var printerExist = await _context.Printers.FindAsync(id);
		if (printerExist == null) return RepositoryResponse<bool>.Failure(new List<string> { "" }, $"Принтер {id} не найден");
		try
		{
			_context.Printers.Remove(printerExist);
			await _context.SaveChangesAsync();
			return RepositoryResponse<bool>.Success(true, "Принтер удален");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<bool>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<List<Printer>>> GetAllAsync()
	{
		try
		{
			var printers = await _context.Printers.AsNoTracking().ToListAsync();
			return RepositoryResponse<List<Printer>>.Success(printers, $"Получено {printers.Count} записей");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<List<Printer>>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<Printer>> GetByIdAsync(int id)
	{
		if (id <= 0) return new RepositoryResponse<Printer>().HandleException(new ArgumentNullException(nameof(id)));
		try
		{
			var result = await _context.Printers
							.Include(p => p.Location)
							.Include(p => p.PrintModel)
							.Where(p => p.Id == id)
							.FirstOrDefaultAsync();
			if (result == null) return RepositoryResponse<Printer>.Failure(new List<string>(), $"Не удалось найти принтер с id = {id}");
			return RepositoryResponse<Printer>.Success(result, "Принтер получен");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<Printer>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<Printer>> GetBySerialNumberAsync(string serialNumber)
	{
		if (string.IsNullOrEmpty(serialNumber)) return new RepositoryResponse<Printer>().HandleException(new ArgumentNullException(nameof(serialNumber)));
		try
		{
			var result = await _context.Printers
							.Include(p => p.PrintModel)
							.Include(p => p.Location)
							.Where(p => p.SerialNumber
								.Equals(serialNumber))
							.FirstOrDefaultAsync();
			if (result == null) return RepositoryResponse<Printer>.Failure(new List<string>(), $"Не удалось найти принтер с серийным номером = {serialNumber}");
			return RepositoryResponse<Printer>.Success(result, "Принтер получен");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<Printer>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<Printer>> UpdateAsync(Printer printer)
	{
		if (printer == null) return new RepositoryResponse<Printer>().HandleException(new ArgumentNullException(nameof(printer)));
		var printerExist = await _context.Printers.FindAsync(printer.Id);
		if (printerExist == null) return RepositoryResponse<Printer>.Failure(new List<string> { "" }, "Не найден изменяемый объект");
		try
		{
			printerExist.Title = printer.Title;
			printerExist.IpAddress = printer.IpAddress;
			printerExist.PrintModelId = printer.PrintModelId;
			_context.Printers.Update(printer);
			await _context.SaveChangesAsync();
			return RepositoryResponse<Printer>.Success(printerExist, "Принтер обновлен");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<Printer>().HandleException(ex);
		}
	}
}