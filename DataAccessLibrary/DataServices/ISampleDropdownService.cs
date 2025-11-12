using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataAccessLibrary.DataServices
{
    public interface ISampleDropdownService
    {
        Task<List<SelectListItem>> GetAllActiveSelectListAsync();
        Task<string> GetTitleById(int? id);
    }
}