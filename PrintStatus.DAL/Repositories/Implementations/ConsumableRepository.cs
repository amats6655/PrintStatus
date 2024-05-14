using Microsoft.EntityFrameworkCore;
using PrintStatus.DAL.Data;
using PrintStatus.DAL.Repositories.Interfaces;
using PrintStatus.DAL.Responses;
using PrintStatus.DOM.Models;

namespace PrintStatus.DAL.Repositories.Implementations;

public class ConsumableRepository(AppDbContext appDbContext) : IConsumableRepository
{
	public async Task<IRepositoryResponse<Consumable>> InsertAsync(Consumable? consumable)
	{
		if (consumable is null) return new RepositoryResponse<Consumable>().HandleException(new ArgumentNullException(nameof(consumable)));
		var consumableExist = await appDbContext.Consumables.AnyAsync(l => l.PrintOidId.Equals(consumable.PrintOidId) && l.PrintModelId == consumable.PrintModelId);
		if (consumableExist) return RepositoryResponse<Consumable>.Failure(new List<string>(), $"{consumable.Name} уже существует");
		try
		{
			await appDbContext.AddAsync(consumable);
			await appDbContext.SaveChangesAsync();
			return RepositoryResponse<Consumable>.Success(consumable, $"{consumable.Name} создан");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<Consumable>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<Consumable>> UpdateAsync(Consumable consumable)
	{
		if (consumable is null) return new RepositoryResponse<Consumable>().HandleException(new ArgumentNullException(nameof(consumable)));
		var consumableExist = await appDbContext.Consumables.FindAsync(consumable.Id);
		if (consumableExist == null) return RepositoryResponse<Consumable>.Failure(new List<string> { "" }, $"Не найден изменяемый объект");
		try
		{
			consumableExist.Name = consumable.Name;
			consumableExist.CalcTypeId = consumable.CalcTypeId;
			appDbContext.Update(consumableExist);
			await appDbContext.SaveChangesAsync();
			return RepositoryResponse<Consumable>.Success(consumableExist, "Расходный материал обновлено");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<Consumable>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<Consumable>> GetByIdAsync(int id)
	{
		if (id < 0) return new RepositoryResponse<Consumable>().HandleException(new ArgumentNullException(nameof(id)));
		try
		{
			var result = await appDbContext.Consumables.FindAsync(id);
			if (result == null) return RepositoryResponse<Consumable>.Failure([], $"Не удалось найти id = {id}");
			return RepositoryResponse<Consumable>.Success(result, "Расходный материал получен");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<Consumable>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<List<Consumable>>> GetAllAsync()
	{
		try
		{
			var consumables = await appDbContext.Consumables
								.AsNoTracking()
								.Include(m => m.PrintModel)
								.Include(o => o.PrintOid)
								.ToListAsync();
			return RepositoryResponse<List<Consumable>>.Success(consumables, $"Получено {consumables.Count} записей");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<List<Consumable>>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<bool>> DeleteByIdAsync(int id)
	{
		if (id <= 0) return new RepositoryResponse<bool>().HandleException(new ArgumentNullException(nameof(id)));
		var consumableExist = await appDbContext.Consumables
								.Include(p => p.PrintModel)
								.Include(o => o.PrintOid)
								.FirstOrDefaultAsync(l => l.Id == id);
		if (consumableExist == null) return RepositoryResponse<bool>.Failure(new List<string>(), $"Расходный материал {id} не найдено");
		try
		{
			appDbContext.Remove(consumableExist);
			await appDbContext.SaveChangesAsync();
			return RepositoryResponse<bool>.Success(true, "Расходный материал удален");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<bool>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<Consumable>> GetByNameAsync(string name)
	{
		if (string.IsNullOrEmpty(name))
			return new RepositoryResponse<Consumable>().HandleException(new ArgumentNullException(nameof(name)));
		try
		{
			var result = await appDbContext.Consumables.FirstOrDefaultAsync(n => n.Name!.Equals(name));
			if (result is null) return RepositoryResponse<Consumable>.Failure([], "Not found");
			return RepositoryResponse<Consumable>.Success(result, "Success");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<Consumable>().HandleException(ex);
		}
	}
	
	public async Task<IRepositoryResponse<List<Consumable>>> GetAllByModelIdAsync(int modelId)
	{
		if (modelId <= 0) return new RepositoryResponse<List<Consumable>>().HandleException(new ArgumentNullException(nameof(modelId)));
		try
		{
			var consumables = await appDbContext.PrintModels
				.AsNoTracking()
				.Where(m => m.Id == modelId)
				.SelectMany(o => o.Consumables!)
					.Include(o => o.PrintOid)
				.ToListAsync();
			return RepositoryResponse<List<Consumable>>.Success(consumables, $"Получено {consumables.Count} записей");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<List<Consumable>>().HandleException(ex);
		}
	}
}