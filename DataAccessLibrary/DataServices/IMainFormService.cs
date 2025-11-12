using DataAccessLibrary.Models;

namespace DataAccessLibrary.DataServices
{
    public interface IMainFormService
    {
        Task<int> CreateAsync(MainFormEntityModel mainFormEntityModel, List<MainFormAuditLogEntityModel> auditEntries);
        Task<List<MainFormEntityModel>> GetAll();
        Task<MainFormEntityModel> GetById(int submissionId);
        Task<List<MainFormEntityModel>> GetBySubmittedByWindowsUserName(string windowsName);
        Task<int> GetSampleRadioIdByMainFormId(int submissionId);
    }
}