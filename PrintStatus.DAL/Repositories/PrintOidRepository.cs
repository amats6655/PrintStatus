using Microsoft.EntityFrameworkCore;
using PrintStatus.DAL.Connection;
using PrintStatus.DOM.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.DAL.Repositories
{
	public class PrintOidRepository : IPrintOidRepository
	{
		private readonly ApplicationDbContext _context;

		public PrintOidRepository(ApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<IRepositoryResult<PrintOid>> AddAsync(PrintOid oid)
		{
			if (oid == null) return new RepositoryResult<PrintOid>().HandleException(new ArgumentNullException(nameof(oid)));
			var oidExist = await _context.Oids.AnyAsync(o => o.Value.Equals(oid.Value));
			if (oidExist) return RepositoryResult<PrintOid>.Failure(new List<string>(), $"{oid.Value} уже существует");
			try
			{
				await _context.Oids.AddAsync(oid);
				await _context.SaveChangesAsync();
				return RepositoryResult<PrintOid>.Success(oid, $"Oid {oid.Value} создан");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<PrintOid>().HandleException(ex);
			}
		}
		public async Task<IRepositoryResult<bool>> DeleteAsync(int id)
		{
			if (id <= 0) return new RepositoryResult<bool>().HandleException(new ArgumentNullException(nameof(id)));
			var oidExist = await _context.Oids.FindAsync(id);
			if (oidExist == null) return RepositoryResult<bool>.Failure(new List<string>(), "Oid не найден");
			try
			{
				_context.Oids.Remove(oidExist);
				await _context.SaveChangesAsync();
				return RepositoryResult<bool>.Success(true, "Oid удален");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<bool>().HandleException(ex);
			}
		}
		public async Task<IRepositoryResult<IEnumerable<PrintOid>>> GetAllAsync()
		{
			try
			{
				var oids = await _context.Oids.AsNoTracking().ToListAsync();
				return RepositoryResult<IEnumerable<PrintOid>>.Success(oids, $"Получено {oids.Count} объектов");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<IEnumerable<PrintOid>>().HandleException(ex);
			}
		}
		public async Task<IRepositoryResult<PrintOid>> GetByIdAsync(int id)
		{
			if (id <= 0) return new RepositoryResult<PrintOid>().HandleException(new ArgumentNullException(nameof(id)));
			try
			{
				var result = await _context.Oids.FindAsync(id);
				if (result == null) return RepositoryResult<PrintOid>.Failure(new List<string> {""},  $"Не удалось найти Oid с id = {id}" );
				return RepositoryResult<PrintOid>.Success(result, "Oid найден");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<PrintOid>().HandleException(ex);
			}
		}
		public async Task<IRepositoryResult<PrintOid>> UpdateAsync(PrintOid oid)
		{
			if (oid == null) return new RepositoryResult<PrintOid>().HandleException(new ArgumentNullException(nameof(oid)));
			var oidExist = await _context.Oids.FindAsync(oid.Id);
			if (oidExist == null) return RepositoryResult<PrintOid>.Failure(new List<string>(), "Не найден изменяемый объект");
			try
			{
				oidExist.Value = oid.Value;
				oidExist.Title = oid.Title;
				oidExist.PollingDate = oid.PollingDate;
				_context.Oids.Update(oidExist);
				await _context.SaveChangesAsync();
				return RepositoryResult<PrintOid>.Success(oidExist, "Oid обновлен");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<PrintOid>().HandleException(ex);
			}
		}
		public async Task<IRepositoryResult<IEnumerable<PrintOid>>> GetAllByModelIdAsync(int modelId)
		{
			if (modelId <= 0) return new RepositoryResult<IEnumerable<PrintOid>>().HandleException(new ArgumentNullException(nameof(modelId)));
			try
			{
				var oids = await _context.PrintModels
									.AsNoTracking()
									.Where(m => m.Id == modelId)
									.SelectMany(o => o.Oids)
									.ToListAsync();
				return RepositoryResult<IEnumerable<PrintOid>>.Success(oids, $"Получено {oids.Count} записей");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<IEnumerable<PrintOid>>().HandleException(ex);
			}
		}
	}
}
