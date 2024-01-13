namespace PrintStatus.BLL.DTO
{
	public class OidDTO
	{
		public int Id { get; set; }
		public string? Title { get; set; }
		public string? Value { get; set; }
		public string? Result { get; set; }
		public int PollingDate { get; set; }
		public OidDTO() { }
	}
}
