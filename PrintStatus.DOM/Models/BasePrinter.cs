namespace PrintStatus.DOM.Models
{
	public class BasePrinter
	{
		public int Id { get; set; }
		public string? Title { get; set; }
		public required int PrintModelId { get; set; }
		public required string IpAddress { get; set; }
		public required string SerialNumber { get; set; }
		public int LocationId { get; set; }
		public List<History>? Histories { get; set; }
		public required List<UserProfile> UserProfiles { get; set; }
		public required List<AuditLog> AuditLogs { get; set; }
	}
}
