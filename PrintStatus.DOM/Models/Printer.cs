using System.Text.Json.Serialization;

namespace PrintStatus.DOM.Models;

public class Printer : BaseModel
{
	public int PrintModelId { get; set; }
	public required string IpAddress { get; set; }
	public required string SerialNumber { get; set; }
	public int LocationId { get; set; }
	[JsonIgnore]
	public List<Journal>? Histories { get; set; }
	public Location? Location { get; set; }
	public PrintModel? PrintModel { get; set; }
	[JsonIgnore] 
	public List<ApplicationUser>? ApplicationUsers { get; set; }
}

