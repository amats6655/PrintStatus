using System.ComponentModel.DataAnnotations;

namespace PrintStatus.BLL;

public class LocationDTO
{
	[Required]
	[MinLength(3)]
	[MaxLength(50)]
	public string? Name { get; set; }
}