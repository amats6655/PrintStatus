using System.ComponentModel.DataAnnotations;

namespace PrintStatus.BLL.DTO
{
	public class OidDTO
	{
		public int Id { get; set; }
		[Required]
		[StringLength(50, MinimumLength = 3)]
		[DataType(DataType.Text)]
		public required string Title { get; set; }
		[Required]
		[StringLength(50, MinimumLength = 10)]
		[DataType(DataType.Text)]
		public required string Value { get; set; }
		public string? Result { get; set; } = null;
		[Required]
		public int PollingDate { get; set; }
		public OidDTO() { }
	}
}
