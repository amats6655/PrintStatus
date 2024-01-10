using PrintStatus.BLL.Interfaces;

namespace PrintStatus.BLL.Services
{
    public class AuditLogManagementService : IAuditLogManagementService
    {
        public Task<bool> AuditCreateLog(object model, int UserId)
        {
            throw new NotImplementedException();
        }
    }
}
