using EmailLibrary.Models;

namespace EmailLibrary.Services
{
    public interface IEmailService
    {
        Task SendAdminCreateNotificationEmailAsync(FormSubmissionEmailNotificationAdminModel submissionModel);
        Task SendUserCreateNotificationEmailAsync(FormSubmissionEmailNotificationUserModel submissionModel);
    }
}