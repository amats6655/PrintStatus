using PrintStatus.DAL.Repositories.Interfaces;

namespace PrintStatus.BLL.Services.Implementations;

using AutoMapper;
using DOM.Models;
using DTO;
using Helpers;
using Interfaces;
using DAL.Repositories.Implementations;

public class CalcTypeService : ICalcTypeService
{
	private readonly ICalcTypeRepository _calcTypeRepo;
	private readonly IMapper _mapper;

	public CalcTypeService(ICalcTypeRepository calcTypeRepo, IMapper mapper)
	{
		_calcTypeRepo = calcTypeRepo;
		_mapper = mapper;
	}

	public async Task<IServiceResponse<bool>> InsertAsync(CalcType calcType)
	{
		var calcTypeExist = await _calcTypeRepo.GetByNameAsync(calcType.Name!);
		if (calcTypeExist.Errors.Any()) return ServiceResponse<bool>.Failure(calcTypeExist.Message);
		if (calcTypeExist.IsSuccess) return ServiceResponse<bool>.Failure("Такой объект уже существует");
		var newCalcType = _mapper.Map<CalcType>(calcType);
		var addCalcTypeResult = await _calcTypeRepo.InsertAsync(newCalcType);
		if (!addCalcTypeResult.IsSuccess) return ServiceResponse<bool>.Failure(addCalcTypeResult.Message);
		return ServiceResponse<bool>.Success(true, addCalcTypeResult.Message);
	}

	public async Task<IServiceResponse<bool>> DeleteAsync(int id)
	{
		var resultDelete = await _calcTypeRepo.DeleteByIdAsync(id);
		if (!resultDelete.IsSuccess) return ServiceResponse<bool>.Failure(resultDelete.Message);
		return ServiceResponse<bool>.Success(true, resultDelete.Message);
	}


	public async Task<IServiceResponse<List<CalcType>>> GetAllAsync()
	{
		var calcTypes = await _calcTypeRepo.GetAllAsync();
		if (!calcTypes.IsSuccess) return ServiceResponse<List<CalcType>>.Failure(calcTypes.Message);
		return ServiceResponse<List<CalcType>>.Success(calcTypes.Data, calcTypes.Message);
	}

	public async Task<IServiceResponse<CalcType>> GetByIdAsync(int id)
	{
		var calcType = await _calcTypeRepo.GetByIdAsync(id);
		if (!calcType.IsSuccess) return ServiceResponse<CalcType>.Failure(calcType.Message);
		return ServiceResponse<CalcType>.Success(calcType.Data, calcType.Message);
	}

	public async Task<IServiceResponse<CalcType>> UpdateAsync(CalcType calcType)
	{
		var resultUpdate = await _calcTypeRepo.UpdateAsync(calcType);
		if (!resultUpdate.IsSuccess) return ServiceResponse<CalcType>.Failure(resultUpdate.Message);
		return ServiceResponse<CalcType>.Success(calcType, resultUpdate.Message);
	}
}

