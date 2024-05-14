namespace PrintStatus.DOM.Models;

using System.Text.Json.Serialization;

public class Location : BaseModel
{
	[JsonIgnore]
	public List<Printer>? Printers { get; set; }
}

