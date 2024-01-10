// Ignore Spelling: Oid

namespace PrintStatus.DOM.Models
{
    public class History
    {
        public int Id { get; set; }
        public int BasePrinterId { get; set; }
        public int OidId { get; set; }
        public required string Value { get; set; }
        public DateTime Date { get; set; }
    }
}
