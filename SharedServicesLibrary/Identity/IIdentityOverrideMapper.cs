using AyrshireCollege.Biis.CommonModelsLibraryStudentExperience;
using DataAccessLibrary.Models;
using SharedViewModelLibrary.Models;

namespace SharedServicesLibrary.Identity
{
    public interface IIdentityOverrideMapper
    {
        IdentityOverrideViewModel EntityToViewModel(IdentityOverrideEntityModel entity);
        Task<ResolvedUserIdentity> GetEffectiveIdentityAsync();
        IdentityOverrideViewModel ResolvedToViewModel(ResolvedUserIdentity entity);
        IdentityOverrideEntityModel ToEntity(IdentityOverrideViewModel viewModel);
    }
}