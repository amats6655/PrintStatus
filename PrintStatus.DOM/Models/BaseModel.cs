namespace PrintStatus.DOM.Models;

using System.ComponentModel.DataAnnotations;

public class BaseModel
{
	public int Id { get; set; }
	[Required]
	public string? Name { get; set; }
}