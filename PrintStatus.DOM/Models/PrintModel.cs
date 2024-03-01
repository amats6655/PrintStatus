namespace PrintStatus.DOM.Models;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class PrintModel : BaseModel
{
	[Required]
	public bool IsColor { get; set; }
	[Required]
	public CalcType? CalcType { get; set; }
	public int CalcTypeId { get; set; }
	public string? Image { get; set; }
	[JsonIgnore]
	public List<Printer>? Printers { get; set; }
	[JsonIgnore]
	public List<PrintOid>? PrintOids { get; set; }
}

