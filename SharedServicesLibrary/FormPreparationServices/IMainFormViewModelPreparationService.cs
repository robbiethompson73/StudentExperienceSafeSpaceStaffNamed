using SharedViewModelLibrary.Models;

namespace SharedServicesLibrary.FormPreparationServices
{
    public interface IMainFormViewModelPreparationService
    {
        Task<MainFormViewModel> PrepareCreateViewModelAsync();
        Task<List<MainFormViewModel>> PrepareListViewModelAsync();
        Task<MainFormViewModel> PreparePopulatedViewModelAsync(int id);
    }
}