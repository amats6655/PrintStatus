namespace PrintStatus.BLL.DTO
{
	public class PrinterDTO
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string? Model { get; set; }
		public int ModelId { get; set; }
		public string IpAddress { get; set; }
		public string? Location { get; set; }
		public int LocationId { get; set; }
		public PrinterDTO()
		{

		}
	}
}
