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
		public async Task<Location> AddAsync(Location location)
		{
			ArgumentNullException.ThrowIfNull(location);
			try
			{
				_context.Add(location);
				await _context.SaveChangesAsync();
				return location;
			}
			catch (Exception ex)
			{
				//TODO Добавить обработчик ошибок
				Console.WriteLine(ex.Message);
				return null;
			}

		}

		public async Task<bool> DeleteAsync(Location location)
		{
			ArgumentNullException.ThrowIfNull(location);
			try
			{
				_context.Remove(location);
				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				//TODO Добавить обработчик ошибок
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		public async Task<IEnumerable<Location>> GetAllAsync()
		{
			try
			{
				return await _context.Locations.ToListAsync();
			}
			catch (Exception ex)
			{
				//TODO Добавить обработчик ошибок
				Console.WriteLine(ex.Message);
				return Enumerable.Empty<Location>();
			}
		}

		public async Task<Location> GetByIdAsync(int id)
		{
			try
			{
				return await _context.Locations.FindAsync(id);
			}
			catch (Exception ex)
			{
				//TODO Добавить обработчик ошибок
				Console.WriteLine(ex.Message);
				return null;
			}
		}

		public async Task<Location> GetByTitle(string title)
		{
			ArgumentException.ThrowIfNullOrEmpty(title);
			return await _context.Locations
									.Where(l => l.Title.Equals(title))
									.FirstOrDefaultAsync();
		}

		public async Task<Location> UpdateAsync(Location location)
		{
			ArgumentNullException.ThrowIfNull(location);
			try
			{
				_context.Update(location);
				await _context.SaveChangesAsync();
				return location;
			}
			catch (Exception ex)
			{
				//TODO Добавить обработчик ошибок
				Console.WriteLine(ex.Message);
				return null;
			}
		}
	}
}
