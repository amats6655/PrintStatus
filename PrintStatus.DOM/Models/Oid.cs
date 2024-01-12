namespace PrintStatus.DOM.Models
{
	public class Oid
	{
		public int Id { get; set; }
		public required string Title { get; set; }
		public required string Value { get; set; }
		public int PollingDate { get; set; }
		public List<PrintModel>? Models { get; set; }
		public List<History> Histories { get; set; }
	}
}
