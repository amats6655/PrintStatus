using AutoMapper;
using PrintStatus.BLL.DTO;
using PrintStatus.DOM.Models;

namespace PrintStatus.BLL.Helpers
{
	public class AppMappingProfile : Profile
	{
		public AppMappingProfile()
		{
			CreateMap<PrinterDTO, BasePrinter>().ReverseMap()
				.ForMember(dest => dest.PrintModel, opt => opt.MapFrom(src => src.PrintModel.Title))
				.ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location.Title));
			CreateMap<PrintOid, OidDTO>().ReverseMap();
			CreateMap<PrintModel, PrintModelDTO>().ReverseMap();
		}
	}
}
