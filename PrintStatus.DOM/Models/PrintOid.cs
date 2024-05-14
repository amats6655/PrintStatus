using System.Text.Json.Serialization;

namespace PrintStatus.DOM.Models;
public class PrintOid : BaseModel
{
	public string? Value { get; set; }
	public int PollingRate { get; set; }
	[JsonIgnore]
	public List<Consumable>? Consumables { get; set; }
}

