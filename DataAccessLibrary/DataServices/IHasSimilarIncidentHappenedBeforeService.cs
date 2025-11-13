using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataAccessLibrary.DataServices
{
    public interface IHasSimilarIncidentHappenedBeforeService
    {
        Task<List<HasSimilarIncidentHappenedBeforeEntityModel>> GetAllActive();
        Task<List<SelectListItem>> GetAllActiveSelectListAsync();
        Task<HasSimilarIncidentHappenedBeforeEntityModel> GetHasSimilarIncidentHappenedBeforeById(int id);
        Task<string> GetTitleById(int id);
    }
}