using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PrintStatus.DOM.Models;
public class PrintModel : BaseModel
{
	[Required]
	public bool IsColor { get; set; }
	public string? Image { get; set; }
	[JsonIgnore]
	public List<Printer>? Printers { get; set; }
	[JsonIgnore]
	public List<Consumable>? Consumables { get; set; }
}

