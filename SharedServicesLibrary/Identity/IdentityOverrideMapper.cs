using AyrshireCollege.Biis.CommonModelsLibraryStudentExperience;
using AyrshireCollege.Biis.PresentationFormattingLibrary.UserName;
using AyrshireCollege.Biis.UserIdentityLibrary.IdentityServices;
using AyrshireCollege.Biis.UserIdentityLibrary.Interfaces;
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
    public class IdentityOverrideMapper : IIdentityOverrideMapper
    {
        private readonly IUserIdentityService _userIdentityService;
        private readonly IIdentityOverrideDataService _identityOverrideDataService;

        public IdentityOverrideMapper(IUserIdentityService userIdentityService,
                                      IIdentityOverrideDataService identityOverrideDataService)
        {
            _userIdentityService = userIdentityService;
            _identityOverrideDataService = identityOverrideDataService;
        }

        public async Task<ResolvedUserIdentity> GetEffectiveIdentityAsync()
        {
            // Retrieve the current user's Windows username from the HttpContext via UserIdentityService
            string currentWindowsName = _userIdentityService.GetUserIdentityFromHttpContext();

            // If no username was found (e.g., anonymous or unauthenticated), return a default empty identity
            if (string.IsNullOrWhiteSpace(currentWindowsName))
            {
                return new ResolvedUserIdentity(); // Return an empty ResolvedUserIdentity object
            }

            // Query the database for an identity override record matching the current Windows username
            IdentityOverrideEntityModel? overrideRecord =
                await _identityOverrideDataService.GetOverrideByRealWindowsUsernameAsync(currentWindowsName);

            // If an override record exists, create and return a ResolvedUserIdentity with overridden properties
            if (overrideRecord != null)
            {
                return new ResolvedUserIdentity
                {
                    Id = overrideRecord.Id,
                    RealWindowsUsername = overrideRecord.RealWindowsUsername,
                    RealFormattedName = overrideRecord.RealFormattedName,
                    EffectiveWindowsUsername = overrideRecord.EffectiveWindowsUsername,
                    EffectiveFormattedName = overrideRecord.EffectiveFormattedName,
                    IsOverridden = true // Mark that this identity is overridden
                };
            }

            // If no override exists, return a ResolvedUserIdentity where Effective = Real,
            // and IsOverridden is false
            return new ResolvedUserIdentity
            {
                Id = null,
                RealWindowsUsername = currentWindowsName,
                RealFormattedName = UserNameFormatter.FormatName(UserNameFormatter.StripDomainPrefix(currentWindowsName)),
                EffectiveWindowsUsername = currentWindowsName,
                EffectiveFormattedName = UserNameFormatter.FormatName(UserNameFormatter.StripDomainPrefix(currentWindowsName)),
                IsOverridden = false
            };
        }


        public IdentityOverrideViewModel ResolvedToViewModel(ResolvedUserIdentity entity)
        {
            if (entity == null) return null;

            return new IdentityOverrideViewModel
            {
                Id = entity.Id,
                RealWindowsUsername = entity.RealWindowsUsername,
                RealFormattedName = entity.RealFormattedName,
                EffectiveWindowsUsername = entity.EffectiveWindowsUsername,
                EffectiveFormattedName = entity.EffectiveFormattedName
            };
        }


        public IdentityOverrideViewModel EntityToViewModel(IdentityOverrideEntityModel entity)
        {
            if (entity == null) return null;

            return new IdentityOverrideViewModel
            {
                Id = entity.Id,
                RealWindowsUsername = UserNameFormatter.StripDomainPrefix(entity.RealWindowsUsername),
                RealFormattedName = entity.RealFormattedName,
                EffectiveWindowsUsername = UserNameFormatter.StripDomainPrefix(entity.EffectiveWindowsUsername),
                EffectiveFormattedName = entity.EffectiveFormattedName
            };
        }

        public IdentityOverrideEntityModel ToEntity(IdentityOverrideViewModel viewModel)
        {
            if (viewModel == null) return null;

            return new IdentityOverrideEntityModel
            {
                Id = viewModel.Id,
                RealWindowsUsername = UserNameFormatter.EnsureDomainPrefix(viewModel.RealWindowsUsername),
                RealFormattedName = viewModel.RealFormattedName,
                EffectiveWindowsUsername = UserNameFormatter.EnsureDomainPrefix(viewModel.EffectiveWindowsUsername),
                EffectiveFormattedName = viewModel.EffectiveFormattedName
            };
        }


    }
}
