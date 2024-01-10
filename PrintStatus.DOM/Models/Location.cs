namespace PrintStatus.DOM.Models
{
    public class Location
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public List<BasePrinter>? Printers { get; set; }
    }
}
