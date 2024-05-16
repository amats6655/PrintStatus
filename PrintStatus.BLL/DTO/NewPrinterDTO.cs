using System.ComponentModel.DataAnnotations;

namespace PrintStatus.BLL.DTO;

public class NewPrinterDTO
{
	[Required]
	[MinLength(3)]
	[MaxLength(50)]
	public string Name { get; set; }

	[Required]
	[MinLength(7)]
	[MaxLength(15)]
	[RegularExpression(@"^([0-9]{1,3}\.){3}[0-9]{1,3}$")]
	public string IpAddress { get; set; }
	
	[Required]
	public int LocationId { get; set; }
	
	[Required]
	public int ApplicationUserId { get; set; }
}
