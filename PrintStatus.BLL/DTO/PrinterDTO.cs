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
		
		public PrinterDTO(int id, string title, int modelId, string ipAddress, int locationId)
		{
			Id = id;
			Title = title;
			ModelId = modelId;
			IpAddress = ipAddress;
			LocationId = locationId;
		}
		public PrinterDTO(int id, string title, int modelId, string modelTitle, string ipAddress, int locationId, string locationTitle)
		{
			Id = id;
			Title = title;
			ModelId = modelId;
			Model = modelTitle;
			IpAddress = ipAddress;
			LocationId = locationId;
			Location = locationTitle;
		}
		public PrinterDTO()
		{
			
		}
	}
}
