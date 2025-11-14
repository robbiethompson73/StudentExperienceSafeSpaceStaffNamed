using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataAccessLibrary.DataServices
{
    public interface IIncidentBehaviourTypeService
    {
        Task<int> CreateAsync(IncidentBehaviourTypeEntityModel entityModel);
        Task<List<SelectListItem>> GetAllActiveSelectListAsync();
        Task<List<SelectListItem>> GetAllSelectListAsync();
        Task<string> GetTitleById(int id);
        Task<int> UpdateAsync(IncidentBehaviourTypeEntityModel entityModel);
    }
}