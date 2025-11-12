using SharedViewModelLibrary.Models;

namespace SharedServicesLibrary.EmailServices
{
    public interface IPrepareFormSubmissionEmailNotificationService
    {
        Task PrepareAdminCreateNotificationEmailAsync(MainFormViewModel viewModel, int submissionId);
        Task PrepareUserCreateNotificationEmailAsync(MainFormViewModel viewModel, int submissionId);
    }
}