namespace PrintStatus.BLL.DTO
{
	public class PrinterDTO
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string? PrintModel { get; set; }
		public int PrintModelId { get; set; }
		public string IpAddress { get; set; }
		public string? Location { get; set; }
		public int LocationId { get; set; }
		public PrinterDTO()
		{

		}
	}
}
