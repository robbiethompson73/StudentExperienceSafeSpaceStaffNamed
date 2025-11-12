using DataAccessLibrary.Models;

namespace DataAccessLibrary.DataServices
{
    public interface IAuditLogService
    {
        Task<List<MainFormAuditLogEntityModel>> GetAuditLogsByMainFormIdAsync(int mainFormId);
    }
}