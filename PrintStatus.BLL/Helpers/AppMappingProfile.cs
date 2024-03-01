namespace PrintStatus.BLL.Helpers;

using AutoMapper;
using DOM.Models;
using DTO;

public class AppMappingProfile : Profile
{
	public AppMappingProfile()
	{
		CreateMap<PrinterDTO, Printer>().ReverseMap()
			.ForMember(dest => dest.PrintModel, opt => opt.MapFrom(src => src.PrintModel.Name))
			.ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location.Name));
		CreateMap<PrintOid, OidDTO>().ReverseMap();
		CreateMap<PrintModel, PrintModelDTO>().ReverseMap();
	}
}
