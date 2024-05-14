namespace PrintStatus.BLL.Services.Interfaces;

using Helpers;
using DOM.Models;

public interface ICalcTypeService
{
	Task<IServiceResponse<bool>> InsertAsync(CalcType model);
	Task<IServiceResponse<List<CalcType>>> GetAllAsync();
	Task<IServiceResponse<CalcType>> GetByIdAsync(int id);
	Task<IServiceResponse<bool>> DeleteAsync(int id);
	Task<IServiceResponse<CalcType>> UpdateAsync(CalcType model);
}