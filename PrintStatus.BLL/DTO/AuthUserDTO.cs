using System.ComponentModel.DataAnnotations;

namespace PrintStatus.BLL;

public class AuthUserDTO
{
	[Required]
	[MinLength(5)]
	public string UserName { get; set; }
	[Required]
	[StringLength(100, MinimumLength = 8)]
	[DataType(DataType.Password)]
	public string Password { get; set; }
}
