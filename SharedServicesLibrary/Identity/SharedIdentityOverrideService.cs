using AyrshireCollege.Biis.CommonModelsLibraryStudentExperience;
using DataAccessLibrary.DataServices;
using DataAccessLibrary.Models;
using SharedViewModelLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServicesLibrary.Identity
{
    /// <summary>
    /// A wrapper service that provides access to resolved user identity information,
    /// using the underlying IdentityOverrideMapper implementation.
    /// SharedIdentityOverrideService is the public interface for IdentityOverrideMapper
    /// </summary>
    public class SharedIdentityOverrideService : AyrshireCollege.Biis.UserIdentityLibrary.Interfaces.IIdentityOverrideService
    {
        private readonly IIdentityOverrideDataService _dataAccess;

        // The internal mapper that contains the actual logic for resolving user identity.
        private readonly IIdentityOverrideMapper _mapper;

        /// <summary>
        /// Constructor that accepts the identity override mapper via dependency injection.
        /// </summary>
        /// <param name="mapper">An implementation of IIdentityOverrideMapper that provides the actual identity resolution logic.</param>
        public SharedIdentityOverrideService(IIdentityOverrideDataService dataAccess,
                                             IIdentityOverrideMapper mapper)
        {
            _dataAccess = dataAccess;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the effective identity for the current user.
        /// This method simply delegates to the underlying mapper implementation.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing a resolved user identity object.</returns>
        public Task<ResolvedUserIdentity> GetEffectiveIdentityAsync()
        {
            // Delegate to the underlying mapper without modifying the logic.
            return _mapper.GetEffectiveIdentityAsync();
        }

        public async Task<List<ResolvedUserIdentity>> GetAllOverridesAsync()
        {
            var entities = await _dataAccess.GetIdentityOverridesAll();

            return entities.Select(e => new ResolvedUserIdentity
            {
                Id = e.Id,
                RealWindowsUsername = e.RealWindowsUsername,
                RealFormattedName = e.RealFormattedName,
                EffectiveWindowsUsername = e.EffectiveWindowsUsername,
                EffectiveFormattedName = e.EffectiveFormattedName
            }).ToList();
        }


       




    }
}
