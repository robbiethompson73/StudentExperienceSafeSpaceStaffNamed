using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataAccessLibrary.DataServices
{
    public interface ISampleRadioAdminService
    {
        Task<List<SelectListItem>> GetAllActiveSelectListAsync();
        Task<List<SampleRadioAdminEntityModel>> GetSampleRadioAdminAllActive();
        Task<SampleRadioAdminEntityModel> GetSampleRadioAdminById(int id);
        Task<string> GetTitleById(int id);
    }
}