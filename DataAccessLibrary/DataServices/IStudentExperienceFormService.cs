using DataAccessLibrary.Models;

namespace DataAccessLibrary.DataServices
{
    public interface IStudentExperienceFormService
    {
        Task<List<StudentExperienceFormEntityModel>> GetStudentExperienceFormActive();
    }
}