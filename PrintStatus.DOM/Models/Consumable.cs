using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PrintStatus.DOM.Models;

public class Consumable : BaseModel
{
	public CalcType? CalcType { get; set; }
	public int CalcTypeId { get; set; } = 1;
	public PrintModel? PrintModel { get; set; }
	public int PrintModelId { get; set; }
	public PrintOid? PrintOid { get; set; }
	public int PrintOidId { get; set; }
}