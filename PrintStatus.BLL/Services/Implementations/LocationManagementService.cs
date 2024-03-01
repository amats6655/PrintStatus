namespace PrintStatus.BLL.Services.Implementations;

using AutoMapper;
using DOM.Models;
using DTO;
using Helpers;
using Interfaces;

public class LocationManagementService(
									ILocationRepository locationRepo,
									IMapper mapper
									//IAccountService accountService
									) : ILocationManagementService
{
	private readonly ILocationRepository _locationRepo = locationRepo;
	private readonly IMapper _mapper = mapper;
	//private readonly IAccountService _accountService = accountService;

	public async Task<IServiceResponse<LocationDTO>> AddAsync(LocationDTO location)
	{
		if (location == null) return ServiceResponse<LocationDTO>.Failure("Неверный идентификатор местоположения");
		var locationExist = await _locationRepo.GetByTitleAsync(location.Title);
		if (locationExist.Errors.Any()) return ServiceResponse<LocationDTO>.Failure(locationExist.Message);
		if (locationExist.IsSuccess) return ServiceResponse<LocationDTO>.Failure("Такое местоположение уже существует");
		var newLocation = _mapper.Map<Location>(location);
		var addLocationResult = await _locationRepo.AddAsync(newLocation);
		if (!addLocationResult.IsSuccess) return ServiceResponse<LocationDTO>.Failure(addLocationResult.Message);
		location = _mapper.Map<LocationDTO>(addLocationResult.Data);
		return ServiceResponse<LocationDTO>.Success(location, addLocationResult.Message);

	}

	public async Task<IServiceResponse<bool>> DeleteAsync(int id)
	{
		if (id <= 0) return ServiceResponse<bool>.Failure("Неверный идентификатор местоположения");
		var resultDelete = await _locationRepo.DeleteAsync(id);
		if (!resultDelete.IsSuccess) return ServiceResponse<bool>.Failure(resultDelete.Message);
		return ServiceResponse<bool>.Success(true, resultDelete.Message);
	}

	public async Task<IServiceResponse<IEnumerable<LocationDTO>>> GetAllAsync()
	{
		var locations = await _locationRepo.GetAllAsync();
		if (!locations.IsSuccess) return ServiceResponse<IEnumerable<LocationDTO>>.Failure(locations.Message);
		var result = new List<LocationDTO>();
		foreach (var location in locations.Data)
		{
			result.Add(_mapper.Map<LocationDTO>(location));
		}
		return ServiceResponse<IEnumerable<LocationDTO>>.Success(result, locations.Message);
	}

	public async Task<IServiceResponse<LocationDTO>> GetByIdAsync(int id)
	{
		if (id <= 0) return ServiceResponse<LocationDTO>.Failure("Неверный идентификатор местоположения");
		var location = await _locationRepo.GetByIdAsync(id);
		if (!location.IsSuccess) return ServiceResponse<LocationDTO>.Failure(location.Message);
		var result = _mapper.Map<LocationDTO>(location);
		return ServiceResponse<LocationDTO>.Success(result, location.Message);
	}

	public async Task<IServiceResponse<LocationDTO>> UpdateAsync(LocationDTO locationDTO, string identityUserId)
	{
		if (locationDTO == null) return ServiceResponse<LocationDTO>.Failure("Неверный идентификатор местоположения");
		if (string.IsNullOrEmpty(identityUserId)) return ServiceResponse<LocationDTO>.Failure("Неавторизованная операция");
		//var userRoles = await _accountService.GetRolesAsync(identityUserId);
		//if (!userRoles.IsSuccess) return ServiceResponse<LocationDTO>.Failure(userRoles.Message);
		//if (!userRoles.Data.Any(r => r.Equals("Администратор"))) return ServiceResponse<LocationDTO>.Failure("Недостаточно прав для обновления местоположения");
		Location location = _mapper.Map<Location>(locationDTO);
		var resultUpdate = await _locationRepo.UpdateAsync(location);
		if (!resultUpdate.IsSuccess) return ServiceResponse<LocationDTO>.Failure(resultUpdate.Message);
		locationDTO = _mapper.Map<LocationDTO>(resultUpdate.Data);
		return ServiceResponse<LocationDTO>.Success(locationDTO, resultUpdate.Message);
	}
}

