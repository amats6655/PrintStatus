using AutoMapper;
using PrintStatus.BLL.DTO;
using PrintStatus.BLL.Helpers;
using PrintStatus.BLL.Services.Interfaces;
using PrintStatus.DAL.Repositories.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.BLL.Services.Implementations;

public class ConsumableService : IConsumableService
{
	private readonly IConsumableRepository _consumableRepository;
	private readonly IMapper _mapper;

	public ConsumableService(IConsumableRepository consumableRepository, IMapper mapper)
	{
		_consumableRepository = consumableRepository;
		_mapper = mapper;
	}
	
	
	public async Task<IServiceResponse<bool>> InsertAsync(NewConsumableDTO model)
	{
		var newConsumable = _mapper.Map<Consumable>(model);
		var addConsumableResult = await _consumableRepository.InsertAsync(newConsumable);
		if(!addConsumableResult.IsSuccess) return ServiceResponse<bool>.Failure(addConsumableResult.Message);
		return ServiceResponse<bool>.Success(true, addConsumableResult.Message);
	}

	public async Task<IServiceResponse<List<Consumable>>> GetAllAsync()
	{
		var consumables = await _consumableRepository.GetAllAsync();
		if (!consumables.IsSuccess) return ServiceResponse<List<Consumable>>.Failure(consumables.Message);
		return ServiceResponse<List<Consumable>>.Success(consumables.Data, consumables.Message);
	}

	public async Task<IServiceResponse<Consumable>> GetByIdAsync(int id)
	{
		var consumable = await _consumableRepository.GetByIdAsync(id);
		if(!consumable.IsSuccess) return ServiceResponse<Consumable>.Failure(consumable.Message);
		return ServiceResponse<Consumable>.Success(consumable.Data, consumable.Message);
	}

	public async Task<IServiceResponse<bool>> DeleteAsync(int id)
	{
		var resultDelete = await _consumableRepository.DeleteByIdAsync(id);
		if (!resultDelete.IsSuccess) return ServiceResponse<bool>.Failure(resultDelete.Message);
		return ServiceResponse<bool>.Success(true, resultDelete.Message);
	}

	public async Task<IServiceResponse<Consumable>> UpdateAsync(Consumable model)
	{
		var resultUpdate = await _consumableRepository.UpdateAsync(model);
		if (!resultUpdate.IsSuccess) return ServiceResponse<Consumable>.Failure(resultUpdate.Message);
		return ServiceResponse<Consumable>.Success(model, resultUpdate.Message);
	}
	
	public async Task<IServiceResponse<List<Consumable>>> GetAllByModelAsync(int modelId)
	{
		var consumables = await _consumableRepository.GetAllByModelIdAsync(modelId);
		if (!consumables.IsSuccess) return ServiceResponse<List<Consumable>>.Failure(consumables.Message);
		return ServiceResponse<List<Consumable>>.Success(consumables.Data, consumables.Message);
	}
}