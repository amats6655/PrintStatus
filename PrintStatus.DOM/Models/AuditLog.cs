namespace PrintStatus.DOM.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public required string ActionType { get; set; }
        public required string OldValue { get; set; }
        public required string NewValue { get; set; }
        public DateTime Date { get; set; }
        public required int UserId { get; set; }
    }
}
