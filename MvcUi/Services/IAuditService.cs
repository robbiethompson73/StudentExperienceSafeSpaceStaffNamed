using DataAccessLibrary.Models;

namespace MvcUi.Services
{
    public interface IAuditService
    {
        Task<List<MainFormAuditLogEntityModel>> GenerateAuditLogEntries(MainFormEntityModel original, MainFormEntityModel updated, string changedBy);
        Task<List<MainFormAuditLogEntityModel>> GenerateAuditLogEntriesForCreation(MainFormEntityModel newSubmission, string changedBy);
    }
}