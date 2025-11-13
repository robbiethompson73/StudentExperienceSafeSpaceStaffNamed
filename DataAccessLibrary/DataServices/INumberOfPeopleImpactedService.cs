using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataAccessLibrary.DataServices
{
    public interface INumberOfPeopleImpactedService
    {
        Task<List<NumberOfPeopleImpactedEntityModel>> GetAllActive();
        Task<List<SelectListItem>> GetAllActiveSelectListAsync();
        Task<NumberOfPeopleImpactedEntityModel> GetNumberOfPeopleImpactedById(int id);
        Task<string> GetTitleById(int id);
    }
}