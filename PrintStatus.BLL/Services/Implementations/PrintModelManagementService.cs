namespace PrintStatus.BLL.Services.Implementations;

using AutoMapper;
using DOM.Models;
using DTO;
using Helpers;
using Interfaces;

public class PrintModelManagementService(
										IPrintModelRepository printModelRepo,
										IMapper mapper
										//IAccountService accountService
										) : IPrintModelManagementService
{
	private readonly IPrintModelRepository _printModelRepo = printModelRepo;
	private readonly IMapper _mapper = mapper;
	//private readonly IAccountService _accountService = accountService;

	public async Task<IServiceResponse<PrintModelDTO>> AddAsync(string modelTitle)
	{
		if (string.IsNullOrEmpty(modelTitle)) return ServiceResponse<PrintModelDTO>.Failure("Неверный идентификатор модели");
		var modelExist = await _printModelRepo.GetByModelNameAsync(modelTitle);
		if (modelExist.Errors.Any()) return ServiceResponse<PrintModelDTO>.Failure(modelExist.Message);
		if (modelExist.IsSuccess) return ServiceResponse<PrintModelDTO>.Success(_mapper.Map<PrintModelDTO>(modelExist.Data), "Такая модель уже существует");
		var newPrintModel = new PrintModel { Title = modelTitle };
		var addPrintModelResult = await _printModelRepo.AddAsync(newPrintModel);
		if (!addPrintModelResult.IsSuccess) return ServiceResponse<PrintModelDTO>.Failure(addPrintModelResult.Message);
		var result = _mapper.Map<PrintModelDTO>(addPrintModelResult.Data);
		return ServiceResponse<PrintModelDTO>.Success(result, addPrintModelResult.Message);
	}

	public async Task<IServiceResponse<bool>> DeleteAsync(int id)
	{
		if (id <= 0) return ServiceResponse<bool>.Failure("Неверный идентификатор модели");
		var resultDelete = await _printModelRepo.DeleteAsync(id);
		if (!resultDelete.IsSuccess) return ServiceResponse<bool>.Failure(resultDelete.Message);
		return ServiceResponse<bool>.Success(true, resultDelete.Message);
	}

	public async Task<IServiceResponse<IEnumerable<PrintModelDTO>>> GetAllAsync()
	{
		var models = await _printModelRepo.GetAllAsync();
		if (!models.IsSuccess) return ServiceResponse<IEnumerable<PrintModelDTO>>.Failure(models.Message);
		var result = new List<PrintModelDTO>();
		foreach (var model in models.Data)
		{
			result.Add(_mapper.Map<PrintModelDTO>(model));
		}
		return ServiceResponse<IEnumerable<PrintModelDTO>>.Success(result, models.Message);
	}

	public async Task<IServiceResponse<PrintModelDTO>> GetByIdAsync(int id)
	{
		if (id <= 0) return ServiceResponse<PrintModelDTO>.Failure("Неверный идентификатор модели");
		var model = await _printModelRepo.GetByIdAsync(id);
		if (!model.IsSuccess) return ServiceResponse<PrintModelDTO>.Failure(model.Message);
		var result = _mapper.Map<PrintModelDTO>(model.Data);
		return ServiceResponse<PrintModelDTO>.Success(result, model.Message);
	}

	public async Task<IServiceResponse<PrintModelDTO>> UpdateAsync(PrintModelDTO printerModelDTO, string identityUserId)
	{
		if (printerModelDTO == null) return ServiceResponse<PrintModelDTO>.Failure("Неверный идентификатор модели");
		if (string.IsNullOrEmpty(identityUserId)) return ServiceResponse<PrintModelDTO>.Failure("Неавторизованная операция");
		//var userRoles = await _accountService.GetRolesAsync(identityUserId);
		//if (!userRoles.IsSuccess) return ServiceResponse<PrintModelDTO>.Failure("Неудалось получить роль пользователя");
		//if (!userRoles.Data.Any(r => r.Equals("Администратор"))) return ServiceResponse<PrintModelDTO>.Failure("Недостаточно прав для обновления модели");
		PrintModel printer = _mapper.Map<PrintModel>(printerModelDTO);
		var resultUpdate = await _printModelRepo.UpdateAsync(printer);
		if (!resultUpdate.IsSuccess) return ServiceResponse<PrintModelDTO>.Failure(resultUpdate.Message);
		printerModelDTO = _mapper.Map<PrintModelDTO>(resultUpdate.Data);
		return ServiceResponse<PrintModelDTO>.Success(printerModelDTO, "Модель обновлена");
	}
}

