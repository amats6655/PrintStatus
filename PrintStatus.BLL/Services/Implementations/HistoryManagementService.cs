namespace PrintStatus.BLL.Services.Implementations;

using DOM.Models;
using DTO;
using Helpers;
using Interfaces;
using Microsoft.EntityFrameworkCore.Migrations;

public class HistoryManagementService(IHistoryRepository historyRepo) : IHistoryManagementService
{
	private readonly IHistoryRepository _historyRepo = historyRepo;

	public async Task<IServiceResponse<bool>> AddHistoryAsync(Journal journal)
	{
		if (journal == null) return ServiceResponse<bool>.Failure("Неверный идентификатор истории");
		journal.Date = DateTime.UtcNow;
		var resultAddHistory = await _historyRepo.AddAsync(journal);
		if (!resultAddHistory.IsSuccess) return ServiceResponse<bool>.Failure(resultAddHistory.Message);
		return ServiceResponse<bool>.Success(true, "Добавлена запись в историю");
	}

	public async Task<IServiceResponse<IEnumerable<Journal>>> GetAllByPrinterIdAsync(int printerId)
	{
		if (printerId <= 0) return ServiceResponse<IEnumerable<Journal>>.Failure("Неверный идентификатор принтера");
		var histories = await _historyRepo.GetPrinterHistoriesAsync(printerId);
		if (!histories.IsSuccess) return ServiceResponse<IEnumerable<Journal>>.Failure(histories.Message);
		return ServiceResponse<IEnumerable<Journal>>.Success(histories.Data, "История получена");
	}

	public async Task<IServiceResponse<IEnumerable<Journal>>> GetAllHistoriesAsync()
	{
		var histories = await _historyRepo.GetAllAsync();
		if (!histories.IsSuccess) return ServiceResponse<IEnumerable<Journal>>.Failure(histories.Message);
		return ServiceResponse<IEnumerable<Journal>>.Success(histories.Data, "История получена");
	}

	public async Task<IServiceResponse<Journal>> GetByIdAsync(int id)
	{
		if (id <= 0) return ServiceResponse<Journal>.Failure("Неверный идентификатор истории");
		var history = await _historyRepo.GetByIdAsync(id);
		if (!history.IsSuccess) return ServiceResponse<Journal>.Failure(history.Message);
		return ServiceResponse<Journal>.Success(history.Data, "История получена");
	}

}

