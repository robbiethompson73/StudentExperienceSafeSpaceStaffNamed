using DataAccessLibrary.Models;

namespace SharedServicesLibrary.FormPreparationModels
{
    public interface IViewModelContextBuilder
    {
        Task<ViewModelMappingContext> BuildAsync(int submissionId);
    }
}