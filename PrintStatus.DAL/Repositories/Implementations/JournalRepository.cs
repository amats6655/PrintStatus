namespace PrintStatus.DAL.Repositories.Implementations;

using Data;
using DOM.Models;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Responses;

public class JournalRepository(AppDbContext appDbContext) : IJournalRepository
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

	public async Task<IRepositoryResponse<Journal>> UpdateAsync(Journal item)
	{
		if (item is null)
			return new RepositoryResponse<Journal>().HandleException(new ArgumentNullException(nameof(item)));
		var journalExist = await appDbContext.Journals.FindAsync(item.Id);
		if(journalExist is null) return RepositoryResponse<Journal>.Failure([], $"Журнал {item.Id} не найден");
		journalExist.Date = item.Date;
		journalExist.PrinterId = item.PrinterId;
		journalExist.ConsumableId = item.ConsumableId;
		journalExist.Value = item.Value;
		try
		{
			appDbContext.Update(journalExist);
			await appDbContext.SaveChangesAsync();
			return RepositoryResponse<Journal>.Success(journalExist, "Журнал обновлен");
		}
		catch(Exception ex)
		{
			return new RepositoryResponse<Journal>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<bool>> DeleteByIdAsync(int id)
	{
		if (id <= 0) return new RepositoryResponse<bool>().HandleException(new ArgumentNullException(nameof(id)));
		var journalExist = await appDbContext.Journals.FindAsync(id);
		if(journalExist is null) return RepositoryResponse<bool>.Failure([], $"Журнал {id} не найден");
		try
		{
			appDbContext.Remove(journalExist);
			await appDbContext.SaveChangesAsync();
			return RepositoryResponse<bool>.Success(true, "Журнал удален");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<bool>().HandleException(ex);
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
			if (result == null) return RepositoryResponse<Journal>.Failure([], $"Не удалось найти историю с id = {id}");
			return RepositoryResponse<Journal>.Success(result, "История получена");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<Journal>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<List<Journal>>> GetAllByPrinterIdAsync(int printerId)
	{
		if (printerId <= 0) return new RepositoryResponse<List<Journal>>().HandleException(new ArgumentNullException(nameof(printerId)));
		try
		{
			var journals = await appDbContext.Journals
							.AsNoTracking()
							.Where(h => h.PrinterId == printerId)
							.Include(_ => _.Printer)
							.ToListAsync();
			return RepositoryResponse<List<Journal>>.Success(journals, $"Получено {journals.Count} записей");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<List<Journal>>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<List<Journal>>> GetAllByConsumableIdAsync(int printerId, int consumableId)
	{
		if (printerId <= 0 || consumableId <= 0) return new RepositoryResponse<List<Journal>>().HandleException(new ArgumentNullException(nameof(printerId), nameof(consumableId)));
		try
		{
			var journals = await appDbContext.Journals
							.AsNoTracking()
							.Where(h => h.PrinterId == printerId
									&& h.ConsumableId == consumableId)
							.ToListAsync();
			return RepositoryResponse<List<Journal>>.Success(journals, $"Получено {journals.Count} записей");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<List<Journal>>().HandleException(ex);
		}
	}
}

