namespace PrintStatus.BLL.Helpers;

using AutoMapper;
using DOM.Models;
using DTO;

public class AppMappingProfile : Profile
{
	public AppMappingProfile()
	{
		CreateMap<NewPrinterDTO, Printer>().ReverseMap();
		CreateMap<LocationDTO, Location>().ReverseMap();
		CreateMap<NewPrintOidDTO, PrintOid>();
		CreateMap<NewConsumableDTO, Consumable>();
	}
}
