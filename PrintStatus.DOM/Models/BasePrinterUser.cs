namespace PrintStatus.DOM.Models
{
	public class BasePrinterUser
	{
		public string UserId { get; set; }
		public int BasePrinterId { get; set; }
		public BasePrinter Printer { get; set; }
	}
}
