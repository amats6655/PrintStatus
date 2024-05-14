namespace PrintStatus.DAL.Repositories.Implementations;

using Data;
using DOM.Models;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Responses;

public class LocationRepository(AppDbContext appDbContext) : ILocationRepository
{
	public async Task<IRepositoryResponse<Location>> InsertAsync(Location location)
	{
		if (location is null) return new RepositoryResponse<Location>().HandleException(new ArgumentNullException(nameof(location)));
		var locationExist = await appDbContext.Locations.AnyAsync(l => l.Name.Equals(location.Name));
		if (locationExist) return RepositoryResponse<Location>.Failure(new List<string>(), $"{location.Name} уже существует");
		try
		{
			await appDbContext.AddAsync(location);
			await appDbContext.SaveChangesAsync();
			return RepositoryResponse<Location>.Success(location, $"{location.Name} создан");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<Location>().HandleException(ex);
		}

	}

	public async Task<IRepositoryResponse<bool>> DeleteByIdAsync(int id)
	{
		if (id <= 0) return new RepositoryResponse<bool>().HandleException(new ArgumentNullException(nameof(id)));
		var locationExist = await appDbContext.Locations.Include(p => p.Printers).FirstOrDefaultAsync(l => l.Id == id);
		if (locationExist == null) return RepositoryResponse<bool>.Failure(new List<string>(), $"Местоположение {id} не найдено");
		if (locationExist.Printers == null) return RepositoryResponse<bool>.Failure(new List<string>(), "Неудалось получить список связанных принтеров");
		if (locationExist.Printers.Count > 0) return RepositoryResponse<bool>.Failure(new List<string>(), "Невозможно удалить местоположение, так как существуют связанные принтеры");
		try
		{
			appDbContext.Remove(locationExist);
			await appDbContext.SaveChangesAsync();
			return RepositoryResponse<bool>.Success(true, "Местоположение удалено");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<bool>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<List<Location>>> GetAllAsync()
	{
		try
		{
			var locations = await appDbContext.Locations.AsNoTracking().ToListAsync();
			return RepositoryResponse<List<Location>>.Success(locations, $"Получено {locations.Count} записей");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<List<Location>>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<Location>> GetByIdAsync(int id)
	{
		if (id <= 0) return new RepositoryResponse<Location>().HandleException(new ArgumentNullException(nameof(id)));
		try
		{
			var result = await appDbContext.Locations.FindAsync(id);
			if (result == null) return RepositoryResponse<Location>.Failure([], $"Не удалось найти местоположение с id = {id}");
			return RepositoryResponse<Location>.Success(result, "Местоположение получено");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<Location>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<Location>> GetByNameAsync(string name)
	{
		if (string.IsNullOrEmpty(name)) return new RepositoryResponse<Location>().HandleException(new ArgumentNullException(nameof(name)));
		try
		{
			var result = await appDbContext.Locations
								.Where(l => l.Name!.ToLower().Equals(name.ToLower()))
								.FirstOrDefaultAsync();
			if (result == null) return RepositoryResponse<Location>.Failure([], $"Не удалось найти местоволожение с названием ");
			return RepositoryResponse<Location>.Success(result, "Данные получены");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<Location>().HandleException(ex);
		}
	}

	public async Task<IRepositoryResponse<Location>> UpdateAsync(Location location)
	{
		if (location is null) return new RepositoryResponse<Location>().HandleException(new ArgumentNullException(nameof(location)));
		var locationExist = await appDbContext.Locations.FindAsync(location.Id);
		if (locationExist == null) return RepositoryResponse<Location>.Failure(new List<string> { "" }, $"Не найден изменяемый объект");
		try
		{
			locationExist.Name = location.Name;
			appDbContext.Update(locationExist);
			await appDbContext.SaveChangesAsync();
			return RepositoryResponse<Location>.Success(locationExist, "Местоположение обновлено");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<Location>().HandleException(ex);
		}
	}
}

