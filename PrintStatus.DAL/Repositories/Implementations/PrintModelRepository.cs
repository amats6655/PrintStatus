namespace PrintStatus.DAL.Repositories.Implementations;

using Data;
using DOM.Models;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Responses;

public class PrintModelRepository(AppDbContext appDbContext) : IPrintModelRepository
{
	public async Task<IRepositoryResponse<PrintModel>> InsertAsync(PrintModel model)
	{
		if (model is null) return new RepositoryResponse<PrintModel>().HandleException(new ArgumentNullException(nameof(model)));
		var modelExist = await appDbContext.PrintModels.AnyAsync(m => m.Name!.Equals(model.Name));
		if (modelExist) return RepositoryResponse<PrintModel>.Failure(new List<string>(), $"{model.Name} уже существует");
		try
		{
			await appDbContext.PrintModels.AddAsync(model);
			await appDbContext.SaveChangesAsync();
			return RepositoryResponse<PrintModel>.Success(model, $"{model.Name} создан");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<PrintModel>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<bool>> DeleteByIdAsync(int id)
	{
		if (id <= 0) return new RepositoryResponse<bool>().HandleException(new ArgumentNullException(nameof(id)));
		var modelExist = await appDbContext.PrintModels.Include(p => p.Printers).FirstOrDefaultAsync(p => p.Id == id);
		if (modelExist == null) return RepositoryResponse<bool>.Failure(new List<string>(), "Модель не найдена");
		if (modelExist.Printers == null) return RepositoryResponse<bool>.Failure(new List<string>(), "Неудалось получить список связанных принтеров");
		if (modelExist.Printers.Count != 0) return RepositoryResponse<bool>.Failure(new List<string>(), "Невозможно удалить модель, так как существуют связанные принтеры");
		try
		{
			appDbContext.PrintModels.Remove(modelExist);
			await appDbContext.SaveChangesAsync();
			return RepositoryResponse<bool>.Success(true, "Модель удалена");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<bool>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<List<PrintModel>>> GetAllAsync()
	{
		try
		{
			var models = await appDbContext.PrintModels
				.AsNoTracking()
				.Include(p => p.Printers)
				.Include(c => c.Consumables)
				.ToListAsync();
			return RepositoryResponse<List<PrintModel>>.Success(models, $"Получено {models.Count} записей");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<List<PrintModel>>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<PrintModel>> GetByIdAsync(int id)
	{
		if (id <= 0) return new RepositoryResponse<PrintModel>().HandleException(new ArgumentNullException(nameof(id)));
		try
		{
			var result = await appDbContext.PrintModels
										.Include(c => c.Consumables)
										.Where(p => p.Id == id)
										.FirstOrDefaultAsync();
			if (result == null) return RepositoryResponse<PrintModel>.Failure([], $"Не удалось найти модель с id = {id}");
			return RepositoryResponse<PrintModel>.Success(result, "Модель найдена");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<PrintModel>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<PrintModel>> GetByModelNameAsync(string modelName)
	{
		if (string.IsNullOrEmpty(modelName)) return new RepositoryResponse<PrintModel>().HandleException(new ArgumentNullException(nameof(modelName)));
		try
		{
			var result = await appDbContext.PrintModels
				.Where(m => m.Name == modelName)
				.FirstOrDefaultAsync();
			if (result == null) return RepositoryResponse<PrintModel>.Failure([], $"Не удалось найти модель с именем = {modelName}");
			return RepositoryResponse<PrintModel>.Success(result, "Модель найдена");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<PrintModel>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<PrintModel>> UpdateAsync(PrintModel model)
	{
		if (model is null) return new RepositoryResponse<PrintModel>().HandleException(new ArgumentNullException(nameof(model)));
		var modelExist = await appDbContext.PrintModels.FindAsync(model.Id);
		if (modelExist == null) return RepositoryResponse<PrintModel>.Failure(new List<string>(), "Не найден изменяемый объект");
		try
		{
			modelExist.Name = model.Name;
			modelExist.IsColor = model.IsColor;
			modelExist.Image = model.Image;
			appDbContext.PrintModels.Update(modelExist);
			await appDbContext.SaveChangesAsync();
			return RepositoryResponse<PrintModel>.Success(modelExist, "Модель обновлена");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<PrintModel>().HandleException(ex);
		}
	}
}

