using DataAccessLibrary.Models;
using SharedViewModelLibrary.Models;

namespace SharedServicesLibrary.StudentExperienceFormServices
{
    public interface IStudentExperienceFormMapper
    {
        StudentExperienceFormEntityModel ToEntity(StudentExperienceFormViewModel viewModel);
        StudentExperienceFormViewModel ToViewModel(StudentExperienceFormEntityModel entity);
    }
}