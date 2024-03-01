namespace PrintStatus.DOM.Models;

using System.Text.Json.Serialization;

public class CalcType : BaseModel
{
	[JsonIgnore]
	public List<PrintModel> PrintModels { get; set; }
}