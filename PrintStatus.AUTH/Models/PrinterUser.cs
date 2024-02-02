using PrintStatus.AUTH.Models;
using PrintStatus.DOM.Models;

namespace PrintStatus.AUTH;

public class PrinterUser : BasePrinterUser
{

	public ApplicationUser User { get; set; }
	public BasePrinter Printer { get; set; }
}
