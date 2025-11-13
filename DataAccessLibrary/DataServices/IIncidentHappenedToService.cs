using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataAccessLibrary.DataServices
{
    public interface IIncidentHappenedToService
    {
        Task<List<IncidentHappenedToEntityModel>> GetAllActive();
        Task<List<SelectListItem>> GetAllActiveSelectListAsync();
        Task<IncidentHappenedToEntityModel> GetIncidentHappenedToById(int id);
        Task<string> GetTitleById(int id);
    }
}