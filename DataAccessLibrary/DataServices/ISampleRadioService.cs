using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataAccessLibrary.DataServices
{
    public interface ISampleRadioService
    {
        Task<List<SampleRadioEntityModel>> GetAllActive();
        Task<List<SelectListItem>> GetAllActiveSelectListAsync();
        Task<SampleRadioEntityModel> GetSampleRadioById(int id);
        Task<string> GetTitleById(int id);
    }
}