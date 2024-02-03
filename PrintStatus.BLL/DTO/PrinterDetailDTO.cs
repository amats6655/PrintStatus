namespace PrintStatus.BLL.DTO
{
	public class PrinterDetailDTO : PrinterDTO
	{
		public List<OidDTO> PrintConsumables { get; set; } = [];

	}
}
