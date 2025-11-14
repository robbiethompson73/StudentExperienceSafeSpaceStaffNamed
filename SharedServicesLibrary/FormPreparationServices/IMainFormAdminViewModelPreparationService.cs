using SharedViewModelLibrary.Models;

namespace SharedServicesLibrary.FormPreparationServices
{
    public interface IMainFormAdminViewModelPreparationService
    {
        Task<PagedResult<MainFormAdminViewModel>> PreparePagedListAdminViewModelAsync(string? staffFullName, int? statusId, string? sortBy, string? sortDirection, int? pageNumber, int? pageSize);
        Task<MainFormAdminViewModel> PreparePopulatedAdminViewModelAsync(int id);
    }
}