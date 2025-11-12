using DataAccessLibrary.Models;

namespace DataAccessLibrary.DataServices
{
    public interface IIdentityOverrideDataService
    {
        Task<int?> CreateAsync(IdentityOverrideEntityModel identityOverrideEntityModel);
        Task<int> DeleteById(int id);
        Task<List<IdentityOverrideEntityModel>> GetIdentityOverridesAll();
        Task<IdentityOverrideEntityModel> GetOverrideByIdAsync(int id);
        Task<IdentityOverrideEntityModel> GetOverrideByRealWindowsUsernameAsync(string realWindowsUsername);
        Task<int?> UpdateAsync(IdentityOverrideEntityModel identityOverrideEntityModel);
    }
}