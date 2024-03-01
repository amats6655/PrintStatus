namespace PrintStatus.DAL.Repositories.Interfaces;

using DOM.Models;
using Responses;

public interface IGenericRepositoryInterface<T>
{
	Task<IRepositoryResponse<List<T>>> GetAllAsync();
	Task<IRepositoryResponse<T>> GetByIdAsync(int id);
	Task<IRepositoryResponse<T>> InsertAsync (T item);
	Task<IRepositoryResponse<T>> UpdateAsync (T item);
	Task<IRepositoryResponse<bool>> DeleteByIdAsync (int id);
}


