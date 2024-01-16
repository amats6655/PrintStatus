using Microsoft.EntityFrameworkCore;
using PrintStatus.DAL.Connection;
using PrintStatus.DOM.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.DAL.Repositories
{
	public class LocationRepository : ILocationRepository
	{
		private readonly ApplicationDbContext _context;
		public LocationRepository(ApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<IRepositoryResult<Location>> AddAsync(Location location)
		{
			if (location == null) return new RepositoryResult<Location>().HandleException(new ArgumentNullException(nameof(location)));
			var locationExist = await _context.Locations.AnyAsync(l => l.Title.Equals(location.Title));
			if (locationExist) return RepositoryResult<Location>.Failure(new List<string>(), $"{location.Title} уже существует");
			try
			{
				await _context.AddAsync(location);
				await _context.SaveChangesAsync();
				return RepositoryResult<Location>.Success(location, $"{location.Title} создан");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<Location>().HandleException(ex);
			}

		}

		public async Task<IRepositoryResult<bool>> DeleteAsync(Location location)
		{
			if (location == null) return new RepositoryResult<bool>().HandleException(new ArgumentNullException(nameof(location)));
			var locationExist = await _context.Locations.FindAsync(location.Id);
			if (locationExist == null) return RepositoryResult<bool>.Failure(new List<string>(), $"Местоположение {location.Title} не найдено");
			if (locationExist.Printers.Count != 0) return RepositoryResult<bool>.Failure(new List<string>(), "Невозможно удалить местоположение, так как существуют связанные принтеры");
			try
			{
				_context.Remove(locationExist);
				await _context.SaveChangesAsync();
				return RepositoryResult<bool>.Success(true, "Местоположение удалено");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<bool>().HandleException(ex);
			}
		}

		public async Task<IRepositoryResult<IEnumerable<Location>>> GetAllAsync()
		{
			try
			{
				var locations = await _context.Locations.AsNoTracking().ToListAsync();
				return RepositoryResult<IEnumerable<Location>>.Success(locations, $"Получено {locations.Count} записей");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<IEnumerable<Location>>().HandleException(ex);
			}
		}

		public async Task<IRepositoryResult<Location>> GetByIdAsync(int id)
		{
			if (id <= 0) return new RepositoryResult<Location>().HandleException(new ArgumentNullException(nameof(id)));
			try
			{
				var result = await _context.Locations.FindAsync(id);
				if (result == null) return RepositoryResult<Location>.Failure(new List<string> {""}, $"Не удалось найти местоположение с id = {id}" );
				return RepositoryResult<Location>.Success(result, "Местоположение получено");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<Location>().HandleException(ex);
			}
		}

		public async Task<IRepositoryResult<Location>> GetByTitle(string title)
		{
			if (string.IsNullOrEmpty(title)) return new RepositoryResult<Location>().HandleException(new ArgumentNullException(nameof(title)));
			try
			{
				var result = await _context.Locations
									.Where(l => l.Title.Equals(title))
									.FirstOrDefaultAsync();
				if (result == null) return RepositoryResult<Location>.Failure(new List<string> {""}, $"Не удалось найти местоволожение с названием " );
				return RepositoryResult<Location>.Success(result, "Данные получены");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<Location>().HandleException(ex);
			}
		}

		public async Task<IRepositoryResult<Location>> UpdateAsync(Location location)
		{
			if (location == null) return new RepositoryResult<Location>().HandleException(new ArgumentNullException(nameof(location)));
			var locationExist = await _context.Locations.FindAsync(location.Id);
			if (locationExist == null) return RepositoryResult<Location>.Failure(new List<string> {""}, $"Не найден изменяемый объект" );
			try
			{
				locationExist.Title = location.Title;
				_context.Update(locationExist);
				await _context.SaveChangesAsync();
				return RepositoryResult<Location>.Success(locationExist, "Местоположение обновлено");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<Location>().HandleException(ex);
			}
		}
	}
}
