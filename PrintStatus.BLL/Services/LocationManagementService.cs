using AutoMapper;
using PrintStatus.BLL.Interfaces;
using PrintStatus.DOM.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.BLL.Services
{
	public class LocationManagementService : ILocationManagementService
	{
		private readonly ILocationRepository _locationRepo;
		private readonly IMapper _mapper;

		public LocationManagementService
										(
											ILocationRepository locationRepo,
											IMapper mapper
										)
		{
			_mapper = mapper;
			_locationRepo = locationRepo;
		}

		public async Task<LocationDTO> AddAsync(LocationDTO location)
		{
			ArgumentNullException.ThrowIfNull(location);
			var locationExists = await _locationRepo.GetByTitle(location.Title);
			if (locationExists == null)
			{
				var newLocation = _mapper.Map<Location>(location);
				var resultAdd = await _locationRepo.AddAsync(newLocation);
				return _mapper.Map<LocationDTO>(resultAdd);
			}
			return _mapper.Map<LocationDTO>(locationExists);
		}

		public async Task<bool> DeleteAsync(int id)
		{
			if (id != 0)
			{
				var location = await _locationRepo.GetByIdAsync(id);
				if (location != null && !location.Printers.Any())
				{
					return await _locationRepo.DeleteAsync(location);
				}
			}
			return false;
		}

		public async Task<IEnumerable<LocationDTO>> GetAllAsync()
		{
			var result = new List<LocationDTO>();
			var locations = await _locationRepo.GetAllAsync();

			foreach (var location in locations) result.Add(_mapper.Map<LocationDTO>(location));
			return result;

		}

		public async Task<LocationDTO> GetByIdAsync(int id)
		{
			if (id != 0)
			{
				var dataLocation = await _locationRepo.GetByIdAsync(id);
				return _mapper.Map<LocationDTO>(dataLocation);
			}
			return null;
		}

		public async Task<LocationDTO> UpdateAsync(LocationDTO location)
		{
			ArgumentException.ThrowIfNullOrEmpty(nameof(location));
			var locationExist = await _locationRepo.GetByIdAsync(location.Id);
			if (locationExist != null)
			{
				locationExist.Title = location.Title;
				await _locationRepo.UpdateAsync(locationExist);
				return location;
			}
			return null;
		}
	}
}
