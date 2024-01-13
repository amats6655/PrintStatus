using AutoMapper;
using PrintStatus.BLL.DTO;
using PrintStatus.BLL.Interfaces;
using PrintStatus.DOM.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.BLL.Services
{
	public class OidManagementService : IPrintOidManagementService
	{
		private readonly IPrintOidRepository _oidRepo;
		private readonly IMapper _map;
		public OidManagementService
									(
										IPrintOidRepository oidRepository,
										IMapper map
									)
		{
			_oidRepo = oidRepository;
			_map = map;
		}
		public async Task<OidDTO> AddAsync(OidDTO oid)
		{
			ArgumentNullException.ThrowIfNull(oid);
			var oidExists = await _oidRepo.GetByIdAsync(oid.Id);
			if (oidExists == null)
			{
				var newOid = _map.Map<PrintOid>(oid);
				var resultAdd = await _oidRepo.AddAsync(newOid);
				if (resultAdd != null) return _map.Map<OidDTO>(resultAdd);
			}
			//TODO Добавить обработку null
			return null;
		}

		public async Task<bool> DeleteAsync(int oidId)
		{
			if (oidId != 0)
			{
				var oidExists = await _oidRepo.GetByIdAsync(oidId);
				if (oidExists != null) return await _oidRepo.DeleteAsync(oidExists);
			}
			return false;
		}

		public async Task<IEnumerable<OidDTO>> GetAllByModelAsync(int modelId)
		{
			var result = new List<OidDTO>();
			if (modelId != 0)
			{
				var dataOids = await _oidRepo.GetAllByModelIdAsync(modelId);
				if (dataOids.Any())
				{
					foreach (var oid in dataOids)
					{
						result.Add(_map.Map<OidDTO>(oid));
					}
				}
			}
			return result;
		}

		public async Task<OidDTO> GetByIdAsync(int id)
		{
			var result = await _oidRepo.GetByIdAsync(id)
				?? throw new ArgumentException("Не удалось найти этот объект");
			return _map.Map<OidDTO>(result);
		}

		public async Task<OidDTO> UpdateAsync(OidDTO oid)
		{
			ArgumentException.ThrowIfNullOrEmpty(nameof(oid));
			var oidExist = await _oidRepo.GetByIdAsync(oid.Id);
			if (oidExist != null)
			{
				oidExist.Title = oid.Title;
				oidExist.Value = oid.Value;
				oidExist.PollingDate = oid.PollingDate;
				var resultUpdate = await _oidRepo.UpdateAsync(oidExist);
				return _map.Map<OidDTO>(resultUpdate);
			}
			return null;
		}
	}
}
