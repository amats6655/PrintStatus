namespace PrintStatus.DOM.Models;

public class Journal
{
	public int Id { get; set; }
	public string? Value { get; set; }
	public Printer? Printer { get; set; }
	public int PrinterId { get; set; }
	public PrintOid? PrintOid { get; set; }
	public int PrintOidId { get; set; }
	public DateTime Date { get; set; } = DateTime.UtcNow;
}

