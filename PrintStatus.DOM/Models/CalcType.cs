namespace PrintStatus.DOM.Models;

using System.Text.Json.Serialization;

public class CalcType : BaseModel
{
	[JsonIgnore]
	public List<Consumable>? Consumables { get; set; }
}