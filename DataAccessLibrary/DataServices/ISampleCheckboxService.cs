using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataAccessLibrary.DataServices
{
    public interface ISampleCheckboxService
    {
        Task<int> CreateAsync(SampleCheckboxEntityModel sampleCheckboxEntityModel);
        Task<List<SelectListItem>> GetAllActiveSelectListAsync();
        Task<List<SelectListItem>> GetAllSelectListAsync();
        Task<string> GetTitleById(int id);
        Task<int> UpdateAsync(SampleCheckboxEntityModel entityModel);
    }
}