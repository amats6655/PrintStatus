using System.ComponentModel.DataAnnotations;

namespace PrintStatus.BLL.DTO;

public class LocationDTO
{
	[Required]
	[MinLength(3)]
	[MaxLength(50)]
	public string? Name { get; set; }
}