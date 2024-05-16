using System.ComponentModel.DataAnnotations;

namespace PrintStatus.BLL.DTO;

public class UpdatePrinterDTO
{
	public int Id { get; set; }
	
	[MinLength(3)]
	[MaxLength(50)]
	public string? Name { get; set; }
	
	[MinLength(7)]
	[MaxLength(15)]
	[RegularExpression(@"^([0-9]{1,3}\.){3}[0-9]{1,3}$")]
	public string? IpAddress { get; set; }
	public int LocationId { get; set; }
}