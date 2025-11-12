using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataAccessLibrary.DataServices
{
    public interface IStatusService
    {
        Task<int> CreateAsync(StatusEntityModel statusEntityModel);
        Task<List<SelectListItem>> GetStatusAllActiveSelectListAsync();
        Task<List<SelectListItem>> GetStatusAllSelectListAsync();
        Task<int?> GetStatusIdByTitle(string statusTitle);
        Task<string> GetStatusTitleById(int statusId);
        Task<int> GetSubmittedStatusIdAsync();
        Task<int> UpdateAsync(StatusEntityModel statusEntityModel);
    }
}