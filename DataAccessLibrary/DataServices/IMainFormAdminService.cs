using DataAccessLibrary.Models;

namespace DataAccessLibrary.DataServices
{
    public interface IMainFormAdminService
    {
        Task<(List<MainFormAdminEntityModel> Items, int TotalCount)> GetAllOrByFilterAsync(string? staffFullName, int? statusId, string? sortBy, string? sortDirection, int? pageNumber, int? pageSize);
        Task<MainFormAdminEntityModel?> GetById(int submissionId);
        Task<int?> GetSampleRadioAdminIdByMainFormId(int submissionId);
        Task<int> UpdateAsync(MainFormAdminEntityModel updated, List<MainFormAuditLogEntityModel> auditEntries);
    }
}