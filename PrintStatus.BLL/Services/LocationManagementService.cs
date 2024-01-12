using PrintStatus.BLL.Interfaces;
using PrintStatus.DOM.Interfaces;
using PrintStatus.DOM.Models;
using PrintStatus.BLL.DTO;

namespace PrintStatus.BLL.Services
{
	public class LocationManagementService : ILocationManagementService
	{
		private readonly ILocationRepository _locationRepo;
		
		public LocationManagementService
										(
											ILocationRepository locationRepo
										)
		{
			_locationRepo = locationRepo;
		}
		
		public async Task<LocationDTO> Add(LocationDTO location)
		{
			ArgumentNullException.ThrowIfNull(location);
			var locationExists = await _locationRepo.GetByTitle(location.Title);
			if(locationExists == null)
			{
				var newLocation = new Location(){Title = location.Title, Printers = new List<BasePrinter>()};
				var resultLocatoin = await _locationRepo.AddAsync(newLocation);
				return new LocationDTO(resultLocatoin.Id, resultLocatoin.Title);
			}
			return new LocationDTO(locationExists.Id, locationExists.Title);
		}

		public Task<bool> Delete(int id)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<Location>> GetAll()
		{
			throw new NotImplementedException();
		}

		public Task<LocationDTO> GetById(int id)
		{
			throw new NotImplementedException();
		}

		public Task<LocationDTO> Update(LocationDTO location)
		{
			throw new NotImplementedException();
		}
	}
}
