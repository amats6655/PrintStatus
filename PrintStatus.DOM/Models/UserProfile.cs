namespace PrintStatus.DOM.Models
{
	public class UserProfile
	{
		public int Id { get; set; }
		public required string IdentityId { get; set; }
		public List<BasePrinter>? Printer { get; set; }
	}
}
