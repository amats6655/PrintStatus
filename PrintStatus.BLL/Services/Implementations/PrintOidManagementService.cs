namespace PrintStatus.BLL.Services.Implementations;

using AutoMapper;
using DAL.Repositories.Interfaces;
using DAL.Responses;
using DOM.Models;
using DTO;
using Helpers;
using Interfaces;

public class OidManagementService(
								IGenericRepositoryInterface<PrintOid> oidRepository,
								IMapper mapper,
								//IAccountService accountService,
								IGenericRepositoryInterface<PrintModel> modelRepository
								) : IPrintOidManagementService
{
	public async Task<IServiceResponse<OidDTO>> AddAsync(OidDTO oid, int printModelId)
	{
		if (oid == null || string.IsNullOrEmpty(oid.Value)) return ServiceResponse<OidDTO>.Failure("Неверный идентификатор Oid");
		if (printModelId <= 0) return ServiceResponse<OidDTO>.Failure("Неверный идентификатор принтера");
		var oidExist = await oidRepository.GetByValueAsync(oid.Value);
		if (oidExist.Errors.Any()) return ServiceResponse<OidDTO>.Failure(oidExist.Message);
		var printModel = await modelRepository.GetByIdAsync(printModelId);
		if (!printModel.IsSuccess) return ServiceResponse<OidDTO>.Failure(printModel.Message);
		OidDTO newOidDTO;
		IRepositoryResponse<PrintOid> newOid;
		if (oidExist.IsSuccess)
		{
			if (oidExist.Data.Models != null && oidExist.Data.Models.Any(m => m.Id == printModelId)) return ServiceResponse<OidDTO>.Failure("У этой модели уже есть данный Oid");
			oidExist.Data.Models.Add(printModel.Data);
			newOid = await oidRepository.UpdateAsync(oidExist.Data);
			if (!newOid.IsSuccess) return ServiceResponse<OidDTO>.Failure("Неудалось добавить данный Oid");
			newOidDTO = mapper.Map<OidDTO>(newOid.Data);
		}
		else
		{
			var addOid = mapper.Map<PrintOid>(oid);
			addOid.PrintModels.Add(printModel.Data);
			newOid = await oidRepository.AddAsync(addOid);
			if (!newOid.IsSuccess) return ServiceResponse<OidDTO>.Failure(newOid.Message);
			newOidDTO = mapper.Map<OidDTO>(newOid.Data);
		}
		return ServiceResponse<OidDTO>.Success(newOidDTO, newOid.Message);
	}

	public async Task<IServiceResponse<bool>> DeleteAsync(int oidId)
	{
		if (oidId <= 0) return ServiceResponse<bool>.Failure("Неверный идентификатор Oid");
		var resultDelete = await oidRepository.DeleteAsync(oidId);
		if (!resultDelete.IsSuccess) return ServiceResponse<bool>.Failure(resultDelete.Message);
		return ServiceResponse<bool>.Success(true, resultDelete.Message);
	}

	public async Task<IServiceResponse<IEnumerable<OidDTO>>> GetAllByModelAsync(int modelId)
	{
		if (modelId <= 0) return ServiceResponse<IEnumerable<OidDTO>>.Failure("Неверный идентификатор модели");
		var oids = await oidRepository.GetAllByModelIdAsync(modelId);
		if (!oids.IsSuccess) return ServiceResponse<IEnumerable<OidDTO>>.Failure(oids.Message);
		var result = new List<OidDTO>();
		foreach (var oid in oids.Data)
		{
			result.Add(mapper.Map<OidDTO>(oid));
		}
		return ServiceResponse<IEnumerable<OidDTO>>.Success(result, oids.Message);
	}

	public async Task<IServiceResponse<OidDTO>> GetByIdAsync(int id)
	{
		if (id <= 0) return ServiceResponse<OidDTO>.Failure("Неверный идентификатор");
		var oid = await oidRepository.GetByIdAsync(id);
		if (!oid.IsSuccess) return ServiceResponse<OidDTO>.Failure(oid.Message);
		var result = mapper.Map<OidDTO>(oid.Data);
		return ServiceResponse<OidDTO>.Success(result, oid.Message);
	}

	public async Task<IServiceResponse<OidDTO>> UpdateAsync(OidDTO oid, string identityUserId)
	{
		if (oid == null) return ServiceResponse<OidDTO>.Failure("Неверный идентификатор");
		if (string.IsNullOrEmpty(identityUserId)) return ServiceResponse<OidDTO>.Failure("Неавторизованная операция");
		//var userRoles = await _accountService.GetRolesAsync(identityUserId);
		//if (!userRoles.IsSuccess) return ServiceResponse<OidDTO>.Failure("Неудалось получить роль пользователя");
		//if (!userRoles.Data.Any(r => r.Equals("Администратор"))) return ServiceResponse<OidDTO>.Failure("Недостаточно прав для обновления oid");
		PrintOid printOid = mapper.Map<PrintOid>(oid);
		var resultUpdate = await oidRepository.UpdateAsync(printOid);
		if (!resultUpdate.IsSuccess) return ServiceResponse<OidDTO>.Failure(resultUpdate.Message);
		oid = mapper.Map<OidDTO>(resultUpdate.Data);
		return ServiceResponse<OidDTO>.Success(oid, resultUpdate.Message);
	}
}
