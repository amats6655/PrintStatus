namespace PrintStatus.DAL.Repositories.Implementations;

using Data;
using DOM.Models;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Responses;

public class CalcTypeRepository(AppDbContext appDbContext) : ICalcTypeRepository
{
	public async Task<IRepositoryResponse<CalcType>> InsertAsync(CalcType calcType)
	{
		if (calcType is null) return new RepositoryResponse<CalcType>().HandleException(new ArgumentNullException(nameof(calcType)));
		var calcTypeExist = await appDbContext.CalcTypes.AnyAsync(l => l.Name.Equals(calcType.Name));
		if (calcTypeExist) return RepositoryResponse<CalcType>.Failure(new List<string>(), $"{calcType.Name} уже существует");
		try
		{
			await appDbContext.AddAsync(calcType);
			await appDbContext.SaveChangesAsync();
			return RepositoryResponse<CalcType>.Success(calcType, $"{calcType.Name} создан");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<CalcType>().HandleException(ex);
		}

	}

	public async Task<IRepositoryResponse<bool>> DeleteByIdAsync(int id)
	{
		if (id <= 0) return new RepositoryResponse<bool>().HandleException(new ArgumentNullException(nameof(id)));
		var calcTypeExist = await appDbContext.CalcTypes.Include(p => p.Consumables).FirstOrDefaultAsync(l => l.Id == id);
		if (calcTypeExist == null) return RepositoryResponse<bool>.Failure(new List<string>(), $"Местоположение {id} не найдено");
		if (calcTypeExist.Consumables.Count > 0) return RepositoryResponse<bool>.Failure(new List<string>(), "Невозможно удалить тип, так как существуют связанные расходные материалы");
		try
		{
			appDbContext.Remove(calcTypeExist);
			await appDbContext.SaveChangesAsync();
			return RepositoryResponse<bool>.Success(true, "Тип расходных материалов удален");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<bool>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<CalcType>> GetByNameAsync(string name)
	{
		if (string.IsNullOrEmpty(name))
			return new RepositoryResponse<CalcType>().HandleException(new ArgumentNullException(nameof(name)));
		try
		{
			var result = await appDbContext.CalcTypes.FirstOrDefaultAsync(n => n.Name!.Equals(name));
			if (result == null) return RepositoryResponse<CalcType>.Failure([], "Не найдено");
			return RepositoryResponse<CalcType>.Success(result, "Success");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<CalcType>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<List<CalcType>>> GetAllAsync()
	{
		try
		{
			var calcTypes = await appDbContext.CalcTypes.AsNoTracking().Include(m => m.Consumables).ToListAsync();
			return RepositoryResponse<List<CalcType>>.Success(calcTypes, $"Получено {calcTypes.Count} записей");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<List<CalcType>>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<CalcType>> GetByIdAsync(int id)
	{
		if (id < 0) return new RepositoryResponse<CalcType>().HandleException(new ArgumentNullException(nameof(id)));
		try
		{
			var result = await appDbContext.CalcTypes.FindAsync(id);
			if (result == null) return RepositoryResponse<CalcType>.Failure([], $"Не удалось найти id = {id}");
			return RepositoryResponse<CalcType>.Success(result, "Тип получен");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<CalcType>().HandleException(ex);
		}
	}
	

	public async Task<IRepositoryResponse<CalcType>> UpdateAsync(CalcType calcType)
	{
		if (calcType is null) return new RepositoryResponse<CalcType>().HandleException(new ArgumentNullException(nameof(calcType)));
		var calcTypeExist = await appDbContext.CalcTypes.FindAsync(calcType.Id);
		if (calcTypeExist == null) return RepositoryResponse<CalcType>.Failure(new List<string> { "" }, $"Не найден изменяемый объект");
		try
		{
			calcTypeExist.Name = calcType.Name;
			appDbContext.Update(calcTypeExist);
			await appDbContext.SaveChangesAsync();
			return RepositoryResponse<CalcType>.Success(calcTypeExist, "Местоположение обновлено");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<CalcType>().HandleException(ex);
		}
	}
}

