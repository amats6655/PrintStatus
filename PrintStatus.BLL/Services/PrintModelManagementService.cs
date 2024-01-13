using AutoMapper;
using PrintStatus.BLL.Interfaces;
using PrintStatus.DOM.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.BLL.Services
{
	public class PrintModelManagementService : IPrintModelManagementService
	{
		private readonly IPrintModelRepository _printModelRepo;
		private readonly IMapper _mapper;
		public PrintModelManagementService(IPrintModelRepository printModelRepo, IMapper mapper)
		{
			_printModelRepo = printModelRepo;
			_mapper = mapper;
		}
		public async Task<PrintModelDTO> AddAsync(string modelTitle)
		{
			ArgumentNullException.ThrowIfNull(modelTitle, nameof(modelTitle));
			var modelExists = await _printModelRepo.GetIdByModelNameAsync(modelTitle);
			if (modelExists == null)
			{
				var newModel = new PrintModel { Title = modelTitle, Printers = new List<BasePrinter>() };
				var resultAdd = await _printModelRepo.AddAsync(newModel);
				//TODO Обработать случай, когда resultAdd == null
				return _mapper.Map<PrintModelDTO>(resultAdd);
			}
			return _mapper.Map<PrintModelDTO>(modelExists);
		}

		public async Task<bool> DeleteAsync(int id)
		{
			if (id != 0)
			{
				var modelExists = await _printModelRepo.GetByIdAsync(id);
				if (modelExists != null) return await _printModelRepo.DeleteAsync(modelExists);
			}
			return false;
		}

		public async Task<IEnumerable<PrintModelDTO>> GetAllAsync()
		{
			var result = new List<PrintModelDTO>();
			var modelData = await _printModelRepo.GetAllAsync();
			if (modelData.Any())
			{
				foreach (var model in modelData)
				{
					result.Add(_mapper.Map<PrintModelDTO>(model));
				}
			}
			return result;
		}

		public async Task<PrintModelDTO> GetByIdAsync(int id)
		{
			var result = await _printModelRepo.GetByIdAsync(id)
				?? throw new ArgumentException($"Не удалось найти объект {nameof(PrintStatus)} с ID {id}");
			return _mapper.Map<PrintModelDTO>(result);
		}

		public async Task<PrintModelDTO> UpdateAsync(PrintModelDTO printModel)
		{
			ArgumentException.ThrowIfNullOrEmpty(nameof(printModel));
			var modelExist = await _printModelRepo.GetByIdAsync(printModel.Id);
			if (modelExist != null)
			{
				modelExist.Title = printModel.Title;
				modelExist.IsColor = printModel.IsColor;
				//TODO Как изменить ConsumableCalcType?

				await _printModelRepo.UpdateAsync(modelExist);
				return printModel;
			}
			return null;

		}
	}
}
