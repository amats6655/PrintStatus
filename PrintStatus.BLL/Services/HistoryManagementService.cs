using PrintStatus.BLL.DTO;
using PrintStatus.BLL.Interfaces;
using PrintStatus.DOM.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.BLL.Services
{
	public class HistoryManagementService(IHistoryRepository historyRepo) : IHistoryManagementService
	{
		private readonly IHistoryRepository _historyRepo = historyRepo;

		public async Task<IServiceResult<bool>> AddHistoryAsync(History history)
		{
			if (history == null) return ServiceResult<bool>.Failure("Неверный идентификатор истории");
			history.Date = DateTime.UtcNow;
			var resultAddHistory = await _historyRepo.AddAsync(history);
			if (!resultAddHistory.IsSuccess) return ServiceResult<bool>.Failure(resultAddHistory.Message);
			return ServiceResult<bool>.Success(true, "Добавлена запись в историю");
		}

		public async Task<IServiceResult<IEnumerable<History>>> GetAllByPrinterIdAsync(int printerId)
		{
			if (printerId <= 0) return ServiceResult<IEnumerable<History>>.Failure("Неверный идентификатор принтера");
			var histories = await _historyRepo.GetPrinterHistoriesAsync(printerId);
			if (!histories.IsSuccess) return ServiceResult<IEnumerable<History>>.Failure(histories.Message);
			return ServiceResult<IEnumerable<History>>.Success(histories.Data, "История получена");
		}

		public async Task<IServiceResult<IEnumerable<History>>> GetAllHistoriesAsync()
		{
			var histories = await _historyRepo.GetAllAsync();
			if (!histories.IsSuccess) return ServiceResult<IEnumerable<History>>.Failure(histories.Message);
			return ServiceResult<IEnumerable<History>>.Success(histories.Data, "История получена");
		}

		public async Task<IServiceResult<History>> GetByIdAsync(int id)
		{
			if (id <= 0) return ServiceResult<History>.Failure("Неверный идентификатор истории");
			var history = await _historyRepo.GetByIdAsync(id);
			if (!history.IsSuccess) return ServiceResult<History>.Failure(history.Message);
			return ServiceResult<History>.Success(history.Data, "История получена");
		}

	}
}
