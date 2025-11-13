using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataAccessLibrary.DataServices
{
    public interface INumberOfPeopleCausedIncidentService
    {
        Task<List<NumberOfPeopleCausedIncidentEntityModel>> GetAllActive();
        Task<List<SelectListItem>> GetAllActiveSelectListAsync();
        Task<NumberOfPeopleCausedIncidentEntityModel> GetNumberOfPeopleCausedIncidentById(int id);
        Task<string> GetTitleById(int id);
    }
}