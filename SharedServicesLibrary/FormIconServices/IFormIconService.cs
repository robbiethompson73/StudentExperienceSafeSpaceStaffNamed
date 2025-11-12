using SharedViewModelLibrary.Models;

namespace SharedServicesLibrary.FormIconServices
{
    public interface IFormIconService
    {
        void AssignIconClasses(IEnumerable<StudentExperienceFormViewModel> forms);
        void AssignIconClasses(StudentExperienceFormViewModel form);
    }
}