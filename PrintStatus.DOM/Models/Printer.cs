namespace PrintStatus.DOM.Models;

public class Printer
{
	public int Id { get; set; }
	public string? Title { get; set; }
	public int PrintModelId { get; set; }
	public required string IpAddress { get; set; }
	public required string SerialNumber { get; set; }
	public int LocationId { get; set; }
	public List<Journal>? Histories { get; set; }
	public Location? Location { get; set; }
	public PrintModel? PrintModel { get; set; }
}

