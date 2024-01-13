namespace PrintStatus.DOM.Models
{
    public class PrintModel
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public bool IsColor { get; set; }
        public List<PrintOid> Oids { get; set; }
        public List<BasePrinter>? Printers { get; set; }
        public ConsumableCalcType ConsumableCalcType { get; set; }
    }

    public enum ConsumableCalcType
    {
        LowLevel,
        PageCount,
        Percent
    }
}
