namespace PrintStatus.DAL.Repositories.Implementations;

using Data;
using DOM.Models;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Responses;

public class JournalRepository(AppDbContext appDbContext)
{
	public async Task<IRepositoryResponse<Journal>> InsertAsync(Journal journal)
	{
		if (journal is null) return new RepositoryResponse<Journal>().HandleException(new ArgumentNullException(nameof(journal)));
		try
		{
			await appDbContext.Journals.AddAsync(journal);
			await appDbContext.SaveChangesAsync();
			return RepositoryResponse<Journal>.Success(journal, "Запись создана");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<Journal>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<List<Journal>>> GetAllAsync()
	{
		try
		{
			var journals = await appDbContext.Journals.AsNoTracking().ToListAsync();
			return RepositoryResponse<List<Journal>>.Success(journals, $"Получено {journals.Count} записей");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<List<Journal>>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<Journal>> GetByIdAsync(int id)
	{
		if (id <= 0) return new RepositoryResponse<Journal>().HandleException(new ArgumentNullException(nameof(id)));
		try
		{
			var result = await appDbContext.Journals.FindAsync(id);
			if (result == null) return RepositoryResponse<Journal>.Failure(new List<string> (), $"Не удалось найти историю с id = {id}");
			return RepositoryResponse<Journal>.Success(result, "История получена");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<Journal>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<List<Journal>>> GetPrinterHistoriesAsync(int printerId)
	{
		if (printerId <= 0) return new RepositoryResponse<List<Journal>>().HandleException(new ArgumentNullException(nameof(printerId)));
		try
		{
			var journals = await appDbContext.Journals
							.AsNoTracking()
							.Where(h => h.PrinterId == printerId)
							.ToListAsync();
			return RepositoryResponse<List<Journal>>.Success(journals, $"Получено {journals.Count} записей");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<List<Journal>>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<List<Journal>>> GetPrinterOidHistoriesAsync(int printerId, int oidId)
	{
		if (printerId <= 0 || oidId <= 0) return new RepositoryResponse<List<Journal>>().HandleException(new ArgumentNullException(nameof(printerId), nameof(oidId)));
		try
		{
			var journals = await appDbContext.Journals
							.AsNoTracking()
							.Where(h => h.PrinterId == printerId
									&& h.PrintOidId == oidId)
							.ToListAsync();
			return RepositoryResponse<List<Journal>>.Success(journals, $"Получено {journals.Count} записей");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<List<Journal>>().HandleException(ex);
		}
	}
}

