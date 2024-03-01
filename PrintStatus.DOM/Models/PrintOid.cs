namespace PrintStatus.DOM.Models;

using System.Text.Json.Serialization;

public class PrintOid : BaseModel
{
	public string? Value { get; set; }
	public int PollingRate { get; set; }
	[JsonIgnore]
	public List<PrintModel>? PrintModels { get; set; }
	[JsonIgnore]
	public List<Journal>? Journals { get; set; }
}

