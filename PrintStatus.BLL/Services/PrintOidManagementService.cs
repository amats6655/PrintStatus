using AutoMapper;
using PrintStatus.BLL.DTO;
using PrintStatus.BLL.Interfaces;
using PrintStatus.DOM.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.BLL.Services
{
	public class OidManagementService(
									IPrintOidRepository oidRepository,
									IMapper mapper,
									//IAccountService accountService,
									IPrintModelRepository modelRepository
									) : IPrintOidManagementService
	{
		private readonly IPrintOidRepository _oidRepo = oidRepository;
		private readonly IMapper _mapper = mapper;
		//private readonly IAccountService _accountService = accountService;
		private readonly IPrintModelRepository _modelRepository = modelRepository;

		public async Task<IServiceResult<OidDTO>> AddAsync(OidDTO oid, int printModelId)
		{
			if (oid == null || string.IsNullOrEmpty(oid.Value)) return ServiceResult<OidDTO>.Failure("Неверный идентификатор Oid");
			if (printModelId <= 0) return ServiceResult<OidDTO>.Failure("Неверный идентификатор принтера");
			var oidExist = await _oidRepo.GetByValueAsync(oid.Value);
			if (oidExist.Errors.Any()) return ServiceResult<OidDTO>.Failure(oidExist.Message);
			var printModel = await _modelRepository.GetByIdAsync(printModelId);
			if (!printModel.IsSuccess) return ServiceResult<OidDTO>.Failure(printModel.Message);
			OidDTO newOidDTO;
			IRepositoryResult<PrintOid> newOid;
			if (oidExist.IsSuccess)
			{
				if (oidExist.Data.Models != null && oidExist.Data.Models.Any(m => m.Id == printModelId)) return ServiceResult<OidDTO>.Failure("У этой модели уже есть данный Oid");
				oidExist.Data.Models.Add(printModel.Data);
				newOid = await _oidRepo.UpdateAsync(oidExist.Data);
				if (!newOid.IsSuccess) return ServiceResult<OidDTO>.Failure("Неудалось добавить данный Oid");
				newOidDTO = _mapper.Map<OidDTO>(newOid.Data);
			}
			else
			{
				var addOid = _mapper.Map<PrintOid>(oid);
				addOid.Models.Add(printModel.Data);
				newOid = await _oidRepo.AddAsync(addOid);
				if (!newOid.IsSuccess) return ServiceResult<OidDTO>.Failure(newOid.Message);
				newOidDTO = _mapper.Map<OidDTO>(newOid.Data);
			}
			return ServiceResult<OidDTO>.Success(newOidDTO, newOid.Message);
		}

		public async Task<IServiceResult<bool>> DeleteAsync(int oidId)
		{
			if (oidId <= 0) return ServiceResult<bool>.Failure("Неверный идентификатор Oid");
			var resultDelete = await _oidRepo.DeleteAsync(oidId);
			if (!resultDelete.IsSuccess) return ServiceResult<bool>.Failure(resultDelete.Message);
			return ServiceResult<bool>.Success(true, resultDelete.Message);
		}

		public async Task<IServiceResult<IEnumerable<OidDTO>>> GetAllByModelAsync(int modelId)
		{
			if (modelId <= 0) return ServiceResult<IEnumerable<OidDTO>>.Failure("Неверный идентификатор модели");
			var oids = await _oidRepo.GetAllByModelIdAsync(modelId);
			if (!oids.IsSuccess) return ServiceResult<IEnumerable<OidDTO>>.Failure(oids.Message);
			var result = new List<OidDTO>();
			foreach (var oid in oids.Data)
			{
				result.Add(_mapper.Map<OidDTO>(oid));
			}
			return ServiceResult<IEnumerable<OidDTO>>.Success(result, oids.Message);
		}

		public async Task<IServiceResult<OidDTO>> GetByIdAsync(int id)
		{
			if (id <= 0) return ServiceResult<OidDTO>.Failure("Неверный идентификатор");
			var oid = await _oidRepo.GetByIdAsync(id);
			if (!oid.IsSuccess) return ServiceResult<OidDTO>.Failure(oid.Message);
			var result = _mapper.Map<OidDTO>(oid.Data);
			return ServiceResult<OidDTO>.Success(result, oid.Message);
		}

		public async Task<IServiceResult<OidDTO>> UpdateAsync(OidDTO oid, string identityUserId)
		{
			if (oid == null) return ServiceResult<OidDTO>.Failure("Неверный идентификатор");
			if (string.IsNullOrEmpty(identityUserId)) return ServiceResult<OidDTO>.Failure("Неавторизованная операция");
			//var userRoles = await _accountService.GetRolesAsync(identityUserId);
			//if (!userRoles.IsSuccess) return ServiceResult<OidDTO>.Failure("Неудалось получить роль пользователя");
			//if (!userRoles.Data.Any(r => r.Equals("Администратор"))) return ServiceResult<OidDTO>.Failure("Недостаточно прав для обновления oid");
			PrintOid printOid = _mapper.Map<PrintOid>(oid);
			var resultUpdate = await _oidRepo.UpdateAsync(printOid);
			if (!resultUpdate.IsSuccess) return ServiceResult<OidDTO>.Failure(resultUpdate.Message);
			oid = _mapper.Map<OidDTO>(resultUpdate.Data);
			return ServiceResult<OidDTO>.Success(oid, resultUpdate.Message);
		}
	}
}
