using AutoMapper;
using PrintStatus.BLL.DTO;
using PrintStatus.DOM.Models;

namespace PrintStatus.BLL.Helpers
{
	public class AppMappingProfile : Profile
	{
		public AppMappingProfile()
		{
			CreateMap<BasePrinter, PrinterDTO>().ReverseMap();
			CreateMap<PrintOid, OidDTO>().ReverseMap();
			CreateMap<PrintModel, PrintModelDTO>().ReverseMap();
		}
	}
}
