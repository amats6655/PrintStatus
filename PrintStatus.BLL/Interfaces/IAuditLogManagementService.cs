namespace PrintStatus.BLL.Interfaces
{
    public interface IAuditLogManagementService
    {
        Task<bool> AuditCreateLog(Object model, int UserId);
    }
}
