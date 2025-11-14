using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataAccessLibrary.DataServices
{
    public interface IIncidentMotivationTypeService
    {
        Task<int> CreateAsync(IncidentMotivationTypeEntityModel entityModel);
        Task<List<SelectListItem>> GetAllActiveSelectListAsync();
        Task<List<SelectListItem>> GetAllSelectListAsync();
        Task<string> GetTitleById(int id);
        Task<int> UpdateAsync(IncidentMotivationTypeEntityModel entityModel);
    }
}