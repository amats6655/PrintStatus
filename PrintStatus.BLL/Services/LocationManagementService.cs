using AutoMapper;
using PrintStatus.BLL.DTO;
using PrintStatus.BLL.Interfaces;
using PrintStatus.DOM.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.BLL.Services
{
	public class LocationManagementService(
										ILocationRepository locationRepo,
										IMapper mapper
										//IAccountService accountService
										) : ILocationManagementService
	{
		private readonly ILocationRepository _locationRepo = locationRepo;
		private readonly IMapper _mapper = mapper;
		//private readonly IAccountService _accountService = accountService;

		public async Task<IServiceResult<LocationDTO>> AddAsync(LocationDTO location)
		{
			if (location == null) return ServiceResult<LocationDTO>.Failure("Неверный идентификатор местоположения");
			var locationExist = await _locationRepo.GetByTitleAsync(location.Title);
			if (locationExist.Errors.Any()) return ServiceResult<LocationDTO>.Failure(locationExist.Message);
			if (locationExist.IsSuccess) return ServiceResult<LocationDTO>.Failure("Такое местоположение уже существует");
			var newLocation = _mapper.Map<Location>(location);
			var addLocationResult = await _locationRepo.AddAsync(newLocation);
			if (!addLocationResult.IsSuccess) return ServiceResult<LocationDTO>.Failure(addLocationResult.Message);
			location = _mapper.Map<LocationDTO>(addLocationResult.Data);
			return ServiceResult<LocationDTO>.Success(location, addLocationResult.Message);

		}

		public async Task<IServiceResult<bool>> DeleteAsync(int id)
		{
			if (id <= 0) return ServiceResult<bool>.Failure("Неверный идентификатор местоположения");
			var resultDelete = await _locationRepo.DeleteAsync(id);
			if (!resultDelete.IsSuccess) return ServiceResult<bool>.Failure(resultDelete.Message);
			return ServiceResult<bool>.Success(true, resultDelete.Message);
		}

		public async Task<IServiceResult<IEnumerable<LocationDTO>>> GetAllAsync()
		{
			var locations = await _locationRepo.GetAllAsync();
			if (!locations.IsSuccess) return ServiceResult<IEnumerable<LocationDTO>>.Failure(locations.Message);
			var result = new List<LocationDTO>();
			foreach (var location in locations.Data)
			{
				result.Add(_mapper.Map<LocationDTO>(location));
			}
			return ServiceResult<IEnumerable<LocationDTO>>.Success(result, locations.Message);
		}

		public async Task<IServiceResult<LocationDTO>> GetByIdAsync(int id)
		{
			if (id <= 0) return ServiceResult<LocationDTO>.Failure("Неверный идентификатор местоположения");
			var location = await _locationRepo.GetByIdAsync(id);
			if (!location.IsSuccess) return ServiceResult<LocationDTO>.Failure(location.Message);
			var result = _mapper.Map<LocationDTO>(location);
			return ServiceResult<LocationDTO>.Success(result, location.Message);
		}

		public async Task<IServiceResult<LocationDTO>> UpdateAsync(LocationDTO locationDTO, string identityUserId)
		{
			if (locationDTO == null) return ServiceResult<LocationDTO>.Failure("Неверный идентификатор местоположения");
			if (string.IsNullOrEmpty(identityUserId)) return ServiceResult<LocationDTO>.Failure("Неавторизованная операция");
			//var userRoles = await _accountService.GetRolesAsync(identityUserId);
			//if (!userRoles.IsSuccess) return ServiceResult<LocationDTO>.Failure(userRoles.Message);
			//if (!userRoles.Data.Any(r => r.Equals("Администратор"))) return ServiceResult<LocationDTO>.Failure("Недостаточно прав для обновления местоположения");
			Location location = _mapper.Map<Location>(locationDTO);
			var resultUpdate = await _locationRepo.UpdateAsync(location);
			if (!resultUpdate.IsSuccess) return ServiceResult<LocationDTO>.Failure(resultUpdate.Message);
			locationDTO = _mapper.Map<LocationDTO>(resultUpdate.Data);
			return ServiceResult<LocationDTO>.Success(locationDTO, resultUpdate.Message);
		}
	}
}
