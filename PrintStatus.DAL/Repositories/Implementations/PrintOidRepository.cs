namespace PrintStatus.DAL.Repositories.Implementations;

using Data;
using DOM.Models;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Responses;

public class PrintOidRepository(AppDbContext appDbContext) : IPrintOidRepository
{
	public async Task<IRepositoryResponse<PrintOid>> InsertAsync(PrintOid? oid)
	{
		if (oid is null) return new RepositoryResponse<PrintOid>().HandleException(new ArgumentNullException(nameof(oid)));
		var oidExist = await appDbContext.PrintOids.AnyAsync(o => o.Value.Equals(oid.Value));
		if (oidExist) return RepositoryResponse<PrintOid>.Failure(new List<string>(), $"{oid.Value} уже существует");
		try
		{
			await appDbContext.PrintOids.AddAsync(oid);
			await appDbContext.SaveChangesAsync();
			return RepositoryResponse<PrintOid>.Success(oid, $"Oid {oid.Value} создан");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<PrintOid>().HandleException(ex);
		}
	}
	public async Task<IRepositoryResponse<bool>> DeleteByIdAsync(int id)
	{
		if (id <= 0) return new RepositoryResponse<bool>().HandleException(new ArgumentNullException(nameof(id)));
		var oidExist = await appDbContext.PrintOids.FindAsync(id);
		if (oidExist == null) return RepositoryResponse<bool>.Failure(new List<string>(), "Oid не найден");
		try
		{
			appDbContext.PrintOids.Remove(oidExist);
			await appDbContext.SaveChangesAsync();
			return RepositoryResponse<bool>.Success(true, "Oid удален");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<bool>().HandleException(ex);
		}
	}
	public async Task<IRepositoryResponse<List<PrintOid>>> GetAllAsync()
	{
		try
		{
			var oids = await appDbContext.PrintOids
				.AsNoTracking()
				.ToListAsync();
			return RepositoryResponse<List<PrintOid>>.Success(oids, $"Получено {oids.Count} объектов");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<List<PrintOid>>().HandleException(ex);
		}
	}
	public async Task<IRepositoryResponse<PrintOid>> GetByIdAsync(int id)
	{
		if (id <= 0) return new RepositoryResponse<PrintOid>().HandleException(new ArgumentNullException(nameof(id)));
		try
		{
			var result = await appDbContext.PrintOids.FindAsync(id);
			if (result is null) return RepositoryResponse<PrintOid>.Failure([], $"Не удалось найти Oid с id = {id}");
			return RepositoryResponse<PrintOid>.Success(result, "Oid найден");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<PrintOid>().HandleException(ex);
		}
	}
	public async Task<IRepositoryResponse<PrintOid>> UpdateAsync(PrintOid oid)
	{
		if (oid is null) return new RepositoryResponse<PrintOid>().HandleException(new ArgumentNullException(nameof(oid)));
		var oidExist = await appDbContext.PrintOids.FindAsync(oid.Id);
		if (oidExist == null) return RepositoryResponse<PrintOid>.Failure(new List<string>(), "Не найден изменяемый объект");
		try
		{
			oidExist.Value = oid.Value;
			oidExist.Name = oid.Name;
			oidExist.PollingRate = oid.PollingRate;
			oidExist.Consumables = oid.Consumables;
			appDbContext.PrintOids.Update(oidExist);
			await appDbContext.SaveChangesAsync();
			return RepositoryResponse<PrintOid>.Success(oidExist, "Oid обновлен");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<PrintOid>().HandleException(ex);
		}
	}
	 

	public async Task<IRepositoryResponse<PrintOid>> GetByValueAsync(string value)
	{
		if (string.IsNullOrEmpty(value)) return new RepositoryResponse<PrintOid>().HandleException(new ArgumentNullException(nameof(value))); ;
		try
		{
			var result = await appDbContext.PrintOids
				.Where(v => v.Value!.Equals(value))
				.FirstOrDefaultAsync();
			if (result == null) return RepositoryResponse<PrintOid>.Failure([], $"Не удалось найти Oid с value = {value}");
			return RepositoryResponse<PrintOid>.Success(result, "Oid найден");
		}
		catch (Exception ex)
		{
			return new RepositoryResponse<PrintOid>().HandleException(ex);
		}
	}
}

