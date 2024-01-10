using PrintStatus.DOM.Models;

namespace PrintStatus.DOM.Interfaces
{
    public interface IAuditLogRepository
    {
        Task<AuditLog> GetByIdAsync(int id);
        Task<AuditLog> AddAsync(AuditLog auditLog);
        Task<IEnumerable<AuditLog>> GetAllAsync();
        //Task<IEnumerable<AuditLog>> GetByPrinterAsync(int printerId);
        Task<IEnumerable<AuditLog>> GetByUserAsync(int userId);
        Task<IEnumerable<AuditLog>> GetByDateAsync(DateTime date);
    }
}
