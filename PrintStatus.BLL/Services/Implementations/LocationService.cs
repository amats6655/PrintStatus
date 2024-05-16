using PrintStatus.DAL.Repositories.Interfaces;

namespace PrintStatus.BLL.Services.Implementations;

using AutoMapper;
using DOM.Models;
using DTO;
using Helpers;
using Interfaces;

public class LocationService : ILocationService
{
	private readonly ILocationRepository _locationRepo;
	private readonly IMapper _mapper;

	public LocationService(ILocationRepository locationRepo, IMapper mapper)
	{
		_locationRepo = locationRepo;
		_mapper = mapper;
	}

	public async Task<IServiceResponse<bool>> InsertAsync(LocationDTO location)
	{
		var locationExist = await _locationRepo.GetByNameAsync(location.Name);
		if (locationExist.Errors.Any()) return ServiceResponse<bool>.Failure(locationExist.Message);
		if (locationExist.IsSuccess) return ServiceResponse<bool>.Failure("Такое местоположение уже существует");
		var newLocation = _mapper.Map<Location>(location);
		var addLocationResult = await _locationRepo.InsertAsync(newLocation);
		if (!addLocationResult.IsSuccess) return ServiceResponse<bool>.Failure(addLocationResult.Message);
		return ServiceResponse<bool>.Success(true, addLocationResult.Message);
	}

	public async Task<IServiceResponse<bool>> DeleteAsync(int id)
	{
		var resultDelete = await _locationRepo.DeleteByIdAsync(id);
		if (!resultDelete.IsSuccess) return ServiceResponse<bool>.Failure(resultDelete.Message);
		return ServiceResponse<bool>.Success(true, resultDelete.Message);
	}
	
	public async Task<IServiceResponse<List<Location>>> GetAllAsync()
	{
		var locations = await _locationRepo.GetAllAsync();
		if (!locations.IsSuccess) return ServiceResponse<List<Location>>.Failure(locations.Message);
		return ServiceResponse<List<Location>>.Success(locations.Data, locations.Message);
	}

	public async Task<IServiceResponse<Location>> GetByIdAsync(int id)
	{
		var location = await _locationRepo.GetByIdAsync(id);
		if (!location.IsSuccess) return ServiceResponse<Location>.Failure(location.Message);
		return ServiceResponse<Location>.Success(location.Data, location.Message);
	}

	public async Task<IServiceResponse<Location>> UpdateAsync(Location location)
	{
		var resultUpdate = await _locationRepo.UpdateAsync(location);
		if (!resultUpdate.IsSuccess) return ServiceResponse<Location>.Failure(resultUpdate.Message);
		return ServiceResponse<Location>.Success(location, resultUpdate.Message);
	}
}

