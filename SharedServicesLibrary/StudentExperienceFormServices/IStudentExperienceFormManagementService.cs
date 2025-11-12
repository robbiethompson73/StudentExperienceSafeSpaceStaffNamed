using SharedViewModelLibrary.Models;

namespace SharedServicesLibrary.StudentExperienceFormServices
{
    public interface IStudentExperienceFormManagementService
    {
        Task<List<StudentExperienceFormViewModel>> GetStudentExperienceFormActiveAsync();
    }
}