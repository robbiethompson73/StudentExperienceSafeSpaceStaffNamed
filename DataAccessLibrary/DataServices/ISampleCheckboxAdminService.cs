using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataAccessLibrary.DataServices
{
    public interface ISampleCheckboxAdminService
    {
        Task<int> CreateAsync(SampleCheckboxAdminEntityModel entityModel);
        Task<List<SelectListItem>> GetAllActiveSelectListAsync();
        Task<List<SelectListItem>> GetAllSelectListAsync();
        Task<string> GetTitleById(int id);
        Task<int> UpdateAsync(SampleCheckboxAdminEntityModel entityModel);
    }
}