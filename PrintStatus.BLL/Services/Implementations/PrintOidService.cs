using System.Text;
using AutoMapper;

namespace PrintStatus.BLL.Services.Implementations;

using DAL.Repositories.Interfaces;
using DAL.Responses;
using DOM.Models;
using DTO;
using Helpers;
using Interfaces;
using DAL.Repositories.Implementations;

public class PrintOidService : IPrintOidService
{
	private readonly IPrintOidRepository _printOidRepository;
	private readonly IPrintModelRepository _printModelRepository;
	private readonly IConsumableRepository _consumableRepository;
	private readonly IMapper _mapper;

	public PrintOidService(IPrintOidRepository printOidRepository, IPrintModelRepository printModelRepository, IMapper mapper, IConsumableRepository consumableRepository)
	{
		_printOidRepository = printOidRepository;
		_printModelRepository = printModelRepository;
		_consumableRepository = consumableRepository;
		_mapper = mapper;
	}
	
	
	public async Task<IServiceResponse<bool>> InsertAsync(NewPrintOidDTO oid)
	{
		var oidExist = await _printOidRepository.GetByValueAsync(oid.Value!);
		if (oidExist.Errors.Any()) return ServiceResponse<bool>.Failure(oidExist.Message);
		// var consumable = await _consumableRepository.GetByIdAsync(oid.ConsumableId);
		// if (!consumable.IsSuccess) return ServiceResponse<bool>.Failure(consumable.Message);
		IRepositoryResponse<PrintOid> newOid;
		if (oidExist.IsSuccess)
		{
			oidExist.Data.Name += $" + {oid.Name}";
			newOid = await _printOidRepository.UpdateAsync(oidExist.Data);
			if (!newOid.IsSuccess) return ServiceResponse<bool>.Failure("Не удалось добавить данный Oid");
		}
		else
		{
			var newModel = _mapper.Map<PrintOid>(oid);
			// newModel.Consumables = new List<Consumable> {consumable.Data};
			newOid = await _printOidRepository.InsertAsync(newModel);
			if (!newOid.IsSuccess) return ServiceResponse<bool>.Failure(newOid.Message);
		}
		return ServiceResponse<bool>.Success(true, newOid.Message);
	}
	

	public async Task<IServiceResponse<List<PrintOid>>> GetAllAsync()
	{
		var result = await _printOidRepository.GetAllAsync();
		if (!result.IsSuccess) return ServiceResponse<List<PrintOid>>.Failure(result.Message);
		return ServiceResponse<List<PrintOid>>.Success(result.Data, result.Message);
	}

	public async Task<IServiceResponse<bool>> DeleteAsync(int oidId)
	{
		var resultDelete = await _printOidRepository.DeleteByIdAsync(oidId);
		if (!resultDelete.IsSuccess) return ServiceResponse<bool>.Failure(resultDelete.Message);
		return ServiceResponse<bool>.Success(true, resultDelete.Message);
	}



	public async Task<IServiceResponse<PrintOid>> GetByIdAsync(int id)
	{
		var oid = await _printOidRepository.GetByIdAsync(id);
		if (!oid.IsSuccess) return ServiceResponse<PrintOid>.Failure(oid.Message);
		return ServiceResponse<PrintOid>.Success(oid.Data, oid.Message);
	}

	public async Task<IServiceResponse<PrintOid>> UpdateAsync(PrintOid model)
	{
		var resultUpdate = await _printOidRepository.UpdateAsync(model);
		if (!resultUpdate.IsSuccess) return ServiceResponse<PrintOid>.Failure(resultUpdate.Message);
		return ServiceResponse<PrintOid>.Success(resultUpdate.Data, resultUpdate.Message);
	}
}
