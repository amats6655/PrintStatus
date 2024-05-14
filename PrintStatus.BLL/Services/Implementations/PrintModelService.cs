using PrintStatus.DAL.Repositories.Implementations;
using PrintStatus.DAL.Repositories.Interfaces;

namespace PrintStatus.BLL.Services.Implementations;

using AutoMapper;
using DOM.Models;
using DTO;
using Helpers;
using Interfaces;

public class PrintModelService: IPrintModelService
{
	private readonly IPrintModelRepository _printModelRepo;
	private readonly IMapper _mapper;
	public PrintModelService(IPrintModelRepository printModelRepo, IMapper mapper)
	{
		_printModelRepo = printModelRepo;
		_mapper = mapper;
	}

	public async Task<IServiceResponse<PrintModel>> InsertAsync(string modelName, int calcTypeId)
	{
		var modelExist = await _printModelRepo.GetByModelNameAsync(modelName);
		if (modelExist.Errors.Any()) return ServiceResponse<PrintModel>.Failure(modelExist.Message);
		if (modelExist.IsSuccess) return ServiceResponse<PrintModel>.Success(modelExist.Data, "Такая модель уже существует");
		var model = new PrintModel(){Name = modelName};
		var addPrintModelResult = await _printModelRepo.InsertAsync(model);
		if (!addPrintModelResult.IsSuccess) return ServiceResponse<PrintModel>.Failure(addPrintModelResult.Message);
		return ServiceResponse<PrintModel>.Success(addPrintModelResult.Data, addPrintModelResult.Message);
	}

	public async Task<IServiceResponse<bool>> DeleteAsync(int id)
	{
		var resultDelete = await _printModelRepo.DeleteByIdAsync(id);
		if (!resultDelete.IsSuccess) return ServiceResponse<bool>.Failure(resultDelete.Message);
		return ServiceResponse<bool>.Success(true, resultDelete.Message);
	}

	public async Task<IServiceResponse<List<PrintModel>>> GetAllAsync()
	{
		var models = await _printModelRepo.GetAllAsync();
		if (!models.IsSuccess) return ServiceResponse<List<PrintModel>>.Failure(models.Message);
		return ServiceResponse<List<PrintModel>>.Success(models.Data, models.Message);
	}

	public async Task<IServiceResponse<PrintModel>> GetByIdAsync(int id)
	{
		var model = await _printModelRepo.GetByIdAsync(id);
		if (!model.IsSuccess) return ServiceResponse<PrintModel>.Failure(model.Message);
		return ServiceResponse<PrintModel>.Success(model.Data, model.Message);
	}

	public async Task<IServiceResponse<PrintModel>> UpdateAsync(PrintModel model)
	{
		var resultUpdate = await _printModelRepo.UpdateAsync(model);
		if (!resultUpdate.IsSuccess) return ServiceResponse<PrintModel>.Failure(resultUpdate.Message);
		return ServiceResponse<PrintModel>.Success(resultUpdate.Data, "Модель обновлена");
	}
}

