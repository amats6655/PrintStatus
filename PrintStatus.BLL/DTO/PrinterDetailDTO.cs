namespace PrintStatus.BLL.DTO;

using DOM.Models;
public class PrinterDetailDTO
{
	public int Id { get; set; }
	public string? Name { get; set; }
	public string? IpAddress { get; set; }
	public string? Location { get; set; }
	public string? PrintModel { get; set; }
	public List<PrintOidDTO>? PrintConsumables { get; set; }
}