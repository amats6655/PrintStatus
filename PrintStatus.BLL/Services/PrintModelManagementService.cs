using AutoMapper;
using PrintStatus.BLL.DTO;
using PrintStatus.BLL.Interfaces;
using PrintStatus.DOM.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.BLL.Services
{
	public class PrintModelManagementService : IPrintModelManagementService
	{
		private readonly IPrintModelRepository _printModelRepo;
		private readonly IMapper _mapper;
		private readonly IAccountService _accountService;
		public PrintModelManagementService(
												IPrintModelRepository printModelRepo,
												IMapper mapper,
												IAccountService accountService
											)
		{
			_printModelRepo = printModelRepo;
			_mapper = mapper;
			_accountService = accountService;
		}
		public async Task<IServiceResult<PrintModelDTO>> AddAsync(string modelTitle)
		{
			if (string.IsNullOrEmpty(modelTitle)) return ServiceResult<PrintModelDTO>.Failure("Неверный идентификатор модели");
			var modelExist = await _printModelRepo.GetByModelNameAsync(modelTitle);
			if (!modelExist.Errors.Any()) return ServiceResult<PrintModelDTO>.Failure(modelExist.Message);
			if (modelExist.IsSuccess) return ServiceResult<PrintModelDTO>.Failure("Такое местоположение уже существует");
			var newPrintModel = new PrintModel { Title = modelTitle };
			var addPrintModelResult = await _printModelRepo.AddAsync(newPrintModel);
			if (!addPrintModelResult.IsSuccess) return ServiceResult<PrintModelDTO>.Failure(addPrintModelResult.Message);
			var result = _mapper.Map<PrintModelDTO>(addPrintModelResult.Data);
			return ServiceResult<PrintModelDTO>.Success(result, addPrintModelResult.Message);
		}

		public async Task<IServiceResult<bool>> DeleteAsync(int id)
		{
			if (id <= 0) return ServiceResult<bool>.Failure("Неверный идентификатор модели");
			var modelExist = await _printModelRepo.GetByIdAsync(id);
			if (!modelExist.IsSuccess) return ServiceResult<bool>.Failure(modelExist.Message);
			var resultDelete = await _printModelRepo.DeleteAsync(modelExist.Data);
			if (!resultDelete.IsSuccess) return ServiceResult<bool>.Failure(resultDelete.Message);
			return ServiceResult<bool>.Success(true, resultDelete.Message);
		}

		public async Task<IServiceResult<IEnumerable<PrintModelDTO>>> GetAllAsync()
		{
			var models = await _printModelRepo.GetAllAsync();
			if (!models.IsSuccess) return ServiceResult<IEnumerable<PrintModelDTO>>.Failure(models.Message);
			var result = new List<PrintModelDTO>();
			foreach (var model in models.Data)
			{
				result.Add(_mapper.Map<PrintModelDTO>(model));
			}
			return ServiceResult<IEnumerable<PrintModelDTO>>.Success(result, models.Message);
		}

		public async Task<IServiceResult<PrintModelDTO>> GetByIdAsync(int id)
		{
			if (id <= 0) return ServiceResult<PrintModelDTO>.Failure("Неверный идентификатор модели");
			var model = await _printModelRepo.GetByIdAsync(id);
			if (!model.IsSuccess) return ServiceResult<PrintModelDTO>.Failure(model.Message);
			var result = _mapper.Map<PrintModelDTO>(model.Data);
			return ServiceResult<PrintModelDTO>.Success(result, model.Message);
		}

		public async Task<IServiceResult<PrintModelDTO>> UpdateAsync(PrintModelDTO printerModelDTO, string identityUserId)
		{
			if (printerModelDTO == null) return ServiceResult<PrintModelDTO>.Failure("Неверный идентификатор модели");
			if (string.IsNullOrEmpty(identityUserId)) return ServiceResult<PrintModelDTO>.Failure("Неавторизованная операция");
			var userRoles = await _accountService.GetRolesAsync(identityUserId);
			if (!userRoles.IsSuccess) return ServiceResult<PrintModelDTO>.Failure("Неудалось получить роль пользователя");
			if (!userRoles.Data.Any(r => r.Equals("Администратор"))) return ServiceResult<PrintModelDTO>.Failure("Недостаточно прав для обновления модели");
			PrintModel printer = _mapper.Map<PrintModel>(printerModelDTO);
			var resultUpdate = await _printModelRepo.UpdateAsync(printer);
			if (!resultUpdate.IsSuccess) return ServiceResult<PrintModelDTO>.Failure(resultUpdate.Message);
			printerModelDTO = _mapper.Map<PrintModelDTO>(resultUpdate.Data);
			return ServiceResult<PrintModelDTO>.Success(printerModelDTO, "Модель обновлена");
		}
	}
}
