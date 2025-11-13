using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataAccessLibrary.DataServices
{
    public interface IIncidentLocationService
    {
        Task<List<IncidentLocationEntityModel>> GetAllActive();
        Task<List<SelectListItem>> GetAllActiveSelectListAsync();
        Task<IncidentLocationEntityModel> GetIncidentLocationById(int id);
        Task<string> GetTitleById(int id);
    }
}