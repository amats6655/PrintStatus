using Microsoft.EntityFrameworkCore;
using PrintStatus.DAL.Connection;
using PrintStatus.DOM.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.DAL.Repositories
{
	public class PrintModelRepository : IPrintModelRepository
	{
		private readonly ApplicationDbContext _context;

		public PrintModelRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IRepositoryResult<PrintModel>> AddAsync(PrintModel model)
		{
			if (model == null) return new RepositoryResult<PrintModel>().HandleException(new ArgumentNullException(nameof(model)));
			var modelExist = await _context.PrintModels.AnyAsync(m => m.Title.Equals(model.Title));
			if (modelExist) return RepositoryResult<PrintModel>.Failure(new List<string>(), $"{model.Title} уже существует");
			try
			{
				await _context.PrintModels.AddAsync(model);
				await _context.SaveChangesAsync();
				return RepositoryResult<PrintModel>.Success(model, $"{model.Title} создан");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<PrintModel>().HandleException(ex);
			}
		}

		public async Task<IRepositoryResult<bool>> DeleteAsync(int id)
		{
			if (id <= 0) return new RepositoryResult<bool>().HandleException(new ArgumentNullException(nameof(id)));
			var modelExist = await _context.PrintModels.Include(p => p.Printers).FirstOrDefaultAsync(p => p.Id == id);
			if (modelExist == null) return RepositoryResult<bool>.Failure(new List<string>(),  "Модель не найдена");
			if (modelExist.Printers == null) return RepositoryResult<bool>.Failure(new List<string>(), "Неудалось получить список связанных принтеров");
			if (modelExist.Printers.Count != 0) return RepositoryResult<bool>.Failure(new List<string>(), "Невозможно удалить модель, так как существуют связанные принтеры");
			try
			{
				_context.PrintModels.Remove(modelExist);
				await _context.SaveChangesAsync();
				return RepositoryResult<bool>.Success(true, "Модель удалена");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<bool>().HandleException(ex);
			}
		}

		public async Task<IRepositoryResult<IEnumerable<PrintModel>>> GetAllAsync()
		{
			try
			{
				var models = await _context.PrintModels.AsNoTracking().ToListAsync();
				return RepositoryResult<IEnumerable<PrintModel>>.Success(models, $"Получено {models.Count} записей");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<IEnumerable<PrintModel>>().HandleException(ex);
			}
		}

		public async Task<IRepositoryResult<PrintModel>> GetByIdAsync(int id)
		{
			if (id <= 0) return new RepositoryResult<PrintModel>().HandleException(new ArgumentNullException(nameof(id)));
			try
			{
				var result = await _context.PrintModels.FindAsync(id);
				if (result == null) return RepositoryResult<PrintModel>.Failure(new List<string>(), $"Не удалось найти модель с id = {id}");
				return RepositoryResult<PrintModel>.Success(result, "Модель найдена");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<PrintModel>().HandleException(ex);
			}
		}

		public async Task<IRepositoryResult<PrintModel>> GetByModelNameAsync(string modelName)
		{
			if (string.IsNullOrEmpty(modelName)) return new RepositoryResult<PrintModel>().HandleException(new ArgumentNullException(nameof(modelName)));
			try
			{
				var result = await _context.PrintModels
					.Where(m => m.Title == modelName)
					.FirstOrDefaultAsync();
				if (result == null) return RepositoryResult<PrintModel>.Failure(new List<string>(), $"Не удалось найти модель с именем = {modelName}");
				return RepositoryResult<PrintModel>.Success(result, "Модель найдена");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<PrintModel>().HandleException(ex);
			}
		}

		public async Task<IRepositoryResult<PrintModel>> UpdateAsync(PrintModel model)
		{
			if (model == null) return new RepositoryResult<PrintModel>().HandleException(new ArgumentNullException(nameof(model)));
			var modelExist = await _context.PrintModels.FindAsync(model.Id);
			if (modelExist == null) return RepositoryResult<PrintModel>.Failure(new List<string>(), "Не найден изменяемый объект");
			try
			{
				modelExist.Title = model.Title;
				modelExist.IsColor = model.IsColor;
				modelExist.ConsumableCalcType = model.ConsumableCalcType;
				_context.PrintModels.Update(modelExist);
				await _context.SaveChangesAsync();
				return RepositoryResult<PrintModel>.Success(modelExist, "Модель обновлена");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<PrintModel>().HandleException(ex);
			}
		}
	}
}
