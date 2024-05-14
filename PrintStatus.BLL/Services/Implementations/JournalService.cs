using PrintStatus.DAL.Repositories.Interfaces;

namespace PrintStatus.BLL.Services.Implementations;

using DOM.Models;
using DTO;
using Helpers;
using Interfaces;
using PrintStatus.DAL.Repositories.Implementations;

public class JournalService : IJournalService
{
	private readonly IJournalRepository _journalRepo;
	private readonly IConsumableService _consumableService;

	public JournalService(IJournalRepository journalRepo, IConsumableService consumableService)
	{
		_journalRepo = journalRepo;
		_consumableService = consumableService;
	}
	public async Task<IServiceResponse<bool>> InsertAsync(Journal model)
	{
		model.Date = DateTime.UtcNow;
		var currentJournal = await _journalRepo.GetAllByPrinterIdAsync(model.PrinterId);
		if (!currentJournal.IsSuccess) return ServiceResponse<bool>.Failure(currentJournal.Message);
		
		var previewValue = currentJournal.Data.LastOrDefault(o => o.ConsumableId == model.ConsumableId);
		if (previewValue is null || !previewValue.Value!.Equals(model.Value))
		{
			var resultAddHistory = await _journalRepo.InsertAsync(model);
			if (!resultAddHistory.IsSuccess) return ServiceResponse<bool>.Failure(resultAddHistory.Message);
			var consumable = await _consumableService.GetByIdAsync(model.ConsumableId);
			var updateConsumable = await _consumableService.UpdateAsync(consumable.Data);
			return ServiceResponse<bool>.Success(true, "Добавлена запись в историю");
		}

		return ServiceResponse<bool>.Success(true, "Нет изменений в значении");
	}

	public async Task<IServiceResponse<Journal>> UpdateAsync(Journal model)
	{
		var resultUpdate = await _journalRepo.UpdateAsync(model);
		if (!resultUpdate.IsSuccess) return ServiceResponse<Journal>.Failure(resultUpdate.Message);
		return ServiceResponse<Journal>.Success(resultUpdate.Data, "История обновлена");
	}

	public async Task<IServiceResponse<List<Journal>>> GetAllByPrinterIdAsync(int printerId)
	{
		var allJournals = await _journalRepo.GetAllAsync();
		var result = allJournals.Data.Where(h => h.PrinterId == printerId);
		if (!result.Any()) return ServiceResponse<List<Journal>>.Success(new List<Journal>(), "История не найдена");
		return ServiceResponse<List<Journal>>.Success(result.ToList(), "История получена");
	}
	
	public async Task<IServiceResponse<List<Journal>>> GetAllByOidIdAsync(int printerId, int consumableId)
	{
		var histories = await _journalRepo.GetAllByConsumableIdAsync(printerId, consumableId);
		if (!histories.IsSuccess) return ServiceResponse<List<Journal>>.Failure(histories.Message);
		return ServiceResponse<List<Journal>>.Success(histories.Data, "История получена");
	}

	public async Task<IServiceResponse<List<Journal>>> GetAllAsync()
	{
		var histories = await _journalRepo.GetAllAsync();
		if (!histories.IsSuccess) return ServiceResponse<List<Journal>>.Failure(histories.Message);
		return ServiceResponse<List<Journal>>.Success(histories.Data, "История получена");
	}

	public async Task<IServiceResponse<Journal>> GetByIdAsync(int id)
	{
		if (id <= 0) return ServiceResponse<Journal>.Failure("Неверный идентификатор истории");
		var history = await _journalRepo.GetByIdAsync(id);
		if (!history.IsSuccess) return ServiceResponse<Journal>.Failure(history.Message);
		return ServiceResponse<Journal>.Success(history.Data, "История получена");
	}

	public async Task<IServiceResponse<bool>> DeleteAsync(int id)
	{
		var resultDelete = await _journalRepo.DeleteByIdAsync(id);
		if (!resultDelete.IsSuccess) return ServiceResponse<bool>.Failure(resultDelete.Message);
		return ServiceResponse<bool>.Success(true, resultDelete.Message);
	}
}

