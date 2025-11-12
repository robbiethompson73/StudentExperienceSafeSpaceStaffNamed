using SharedViewModelLibrary.Models;

namespace SharedServicesLibrary.FormPreparationServices
{
    public interface IMainFormAdminViewModelPreparationService
    {
        // ORIG B4 pagination Task<List<MainFormAdminViewModel>> PrepareListAdminViewModelAsync(string? studentReferenceNumber,int? statusId,string? sortBy,string? sortDirection);
        Task<PagedResult<MainFormAdminViewModel>> PreparePagedListAdminViewModelAsync(string? studentReferenceNumber, int? statusId, string? sortBy, string? sortDirection, int? pageNumber, int? pageSize);

        Task<MainFormAdminViewModel> PreparePopulatedAdminViewModelAsync(int id);
    }
}