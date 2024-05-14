namespace PrintStatus.DOM.Models;

public class Journal
{
	public int Id { get; set; }
	public string? Value { get; set; }
	public Printer? Printer { get; set; }
	public int PrinterId { get; set; }
	public Consumable? Consumable { get; set; }
	public int ConsumableId { get; set; }
	public DateTime Date { get; set; } = DateTime.UtcNow;
}

