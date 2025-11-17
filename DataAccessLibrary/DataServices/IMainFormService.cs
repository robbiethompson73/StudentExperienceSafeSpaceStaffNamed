using DataAccessLibrary.Models;

namespace DataAccessLibrary.DataServices
{
    public interface IMainFormService
    {
        Task<int> CreateAsync(MainFormEntityModel mainFormEntityModel, List<MainFormAuditLogEntityModel> auditEntries);
        Task<List<MainFormEntityModel>> GetAll();
        Task<MainFormEntityModel> GetById(int submissionId);
        Task<List<MainFormEntityModel>> GetBySubmittedByWindowsUserName(string windowsName);
        Task<int> GetHasSimilarIncidentHappenedBeforeIdByMainFormId(int submissionId);
        Task<int> GetIncidentHappenedToIdByMainFormId(int submissionId);
        Task<int> GetIncidentLocationIdByMainFormId(int submissionId);
        Task<int> GetNumberOfPeopleCausedIncidentIdByMainFormId(int submissionId);
        Task<int> GetNumberOfPeopleImpactedIdByMainFormId(int submissionId);
    }
}