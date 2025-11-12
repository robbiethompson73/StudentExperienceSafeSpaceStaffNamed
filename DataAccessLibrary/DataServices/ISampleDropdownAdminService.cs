using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataAccessLibrary.DataServices
{
    public interface ISampleDropdownAdminService
    {
        Task<List<SelectListItem>> GetAllActiveSelectListAsync();
        Task<string> GetTitleById(int? id);
    }
}