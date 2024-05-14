namespace PrintStatus.DOM.Models;

using System.Text.Json.Serialization;

public class ApplicationUser
{
	public int Id { get; set; }
	public string? Username { get; set; }
	public string? FullName { get; set; }
	public string? Email { get; set; }
	public string? Password { get; set; }
	[JsonIgnore]
	public List<Printer>? Printers { get; set; }
}

