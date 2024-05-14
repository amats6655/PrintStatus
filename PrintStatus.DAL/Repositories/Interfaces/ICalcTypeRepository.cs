using PrintStatus.DAL.Responses;
using PrintStatus.DOM.Models;

namespace PrintStatus.DAL.Repositories.Interfaces;

public interface ICalcTypeRepository
{
	Task<IRepositoryResponse<CalcType>> InsertAsync(CalcType? oid);
	Task<IRepositoryResponse<CalcType>> UpdateAsync(CalcType oid);
	Task<IRepositoryResponse<CalcType>> GetByIdAsync(int id);
	Task<IRepositoryResponse<List<CalcType>>> GetAllAsync();
	Task<IRepositoryResponse<bool>> DeleteByIdAsync(int id);
	Task<IRepositoryResponse<CalcType>> GetByNameAsync(string name);
}