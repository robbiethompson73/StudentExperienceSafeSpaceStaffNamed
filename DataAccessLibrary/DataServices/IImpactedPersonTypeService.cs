using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataAccessLibrary.DataServices
{
    public interface IImpactedPersonTypeService
    {
        Task<int> CreateAsync(ImpactedPersonTypeEntityModel entityModel);
        Task<List<SelectListItem>> GetAllActiveSelectListAsync();
        Task<List<SelectListItem>> GetAllSelectListAsync();
        Task<string> GetTitleById(int id);
        Task<int> UpdateAsync(ImpactedPersonTypeEntityModel entityModel);
    }
}