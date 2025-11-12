using AyrshireCollege.Biis.CommonModelsLibraryStudentExperience;
using AyrshireCollege.Biis.PresentationFormattingLibrary.UserName;
using AyrshireCollege.Biis.UserIdentityLibrary.Interfaces;
using DataAccessLibrary.DataServices;
using MvcUi.AuthorizationAttributes;
using MvcUi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SharedServicesLibrary.AppSettings;
using SharedServicesLibrary.Identity;
using SharedViewModelLibrary.Models;

namespace MvcUi.Controllers
{
    public class IdentityOverrideController : Controller
    {
        private readonly GlobalSettings _globalSettings;
        private readonly IIdentityOverrideService _identityOverrideService;
        private readonly IIdentityOverrideMapper _identityOverrideMapper;
        private readonly IIdentityOverrideDataService _identityOverrideDataService;

        public IdentityOverrideController(IIdentityOverrideService identityOverrideService,
                                          IIdentityOverrideMapper identityOverrideMapper,
                                          IIdentityOverrideDataService identityOverrideDataService,
                                          IOptions<GlobalSettings> globalSettings)
        {
            _globalSettings = globalSettings.Value;
            _identityOverrideService = identityOverrideService;
            _identityOverrideMapper = identityOverrideMapper;
            _identityOverrideDataService = identityOverrideDataService;
        }


        public async Task<IActionResult> List()
        {
            ViewData["AppName"] = _globalSettings.MyGlobalName;

            // Call GetEffectiveIdentityAsync() through the UserIdentityLibrary service interface
            var resolvedUserIdentityList = await _identityOverrideService.GetAllOverridesAsync();

            var viewModels = resolvedUserIdentityList.Select(x => new IdentityOverrideViewModel
            {
                Id = x.Id ?? 0, // assuming Id is non-nullable in ViewModel
                RealWindowsUsername = UserNameFormatter.StripDomainPrefix(x.RealWindowsUsername),
                RealFormattedName = x.RealFormattedName,
                EffectiveWindowsUsername = UserNameFormatter.StripDomainPrefix(x.EffectiveWindowsUsername),
                EffectiveFormattedName = x.EffectiveFormattedName
            }).ToList();

            return View("List", viewModels);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewData["AppName"] = _globalSettings.MyGlobalName;

            var identityViewModel = new IdentityOverrideViewModel();

            return View("Create", identityViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Create(IdentityOverrideViewModel viewModel)
        {

            if (ModelState.IsValid == false)
            {
                return View("Create", viewModel);
            }

            // Convert ViewModel to EntityModel using IdentityOverrideMapper
            var entityModel = _identityOverrideMapper.ToEntity(viewModel);

            try
            {
                // Attempt to create a new identity override record in the database.
                // This includes checking if a duplicate exists inside the service layer.
                int? newId = await _identityOverrideDataService.CreateAsync(entityModel);

                // If successful, store a message in TempData (persists across the redirect) 
                // to show a success alert on the next page.
                TempData["SuccessMessage"] = "Success";

                // Redirect the user to the List view, optionally passing the new record's ID.
                return RedirectToAction("List", "IdentityOverride", new { Id = newId });
            }
            catch (InvalidOperationException ex)
            {
                // If a duplicate or business-rule violation occurs, add a model-level error.
                // This will be displayed on the Create view as a validation summary or field-level error.
                ModelState.AddModelError("RealWindowsUsername", ex.Message); // "" means general error not tied to a specific property

                // Re-display the Create view with the user's input preserved and error messages shown.
                return View("Create", viewModel);
            }


        }



        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            ViewData["AppName"] = _globalSettings.MyGlobalName;

            // Call GetEffectiveIdentityAsync() through the UserIdentityLibrary service interface
            var entityModel = await _identityOverrideDataService.GetOverrideByIdAsync(id);

            var viewModel = _identityOverrideMapper.EntityToViewModel(entityModel);

            return View("Update", viewModel);
        }



        [HttpPost]
        public async Task<IActionResult> Update(IdentityOverrideViewModel viewModel)
        {
            if (ModelState.IsValid == false)
            {
                return View("Update", viewModel);
            }

            // Convert ViewModel to EntityModel using IdentityOverrideMapper
            var entityModel = _identityOverrideMapper.ToEntity(viewModel);

            // Perform Update
            int? id = await _identityOverrideDataService.UpdateAsync(entityModel);

            // Store a success message in TempData to persist it across the redirect
            TempData["SuccessMessage"] = "Success";

            return RedirectToAction("List", "IdentityOverride", new { Id = id });
        }



        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            ViewData["AppName"] = _globalSettings.MyGlobalName;

            // Call GetEffectiveIdentityAsync() through the UserIdentityLibrary service interface
            var deletedId = await _identityOverrideDataService.DeleteById(id);

            // Store a success message in TempData to persist it across the redirect
            TempData["SuccessMessage"] = "Success";

            return RedirectToAction("List", "IdentityOverride", new { Id = id });
        }






        public async Task<IActionResult> User()
        {
            ViewData["AppName"] = _globalSettings.MyGlobalName;

            // Call GetEffectiveIdentityAsync() through the UserIdentityLibrary service interface
            ResolvedUserIdentity identity = await _identityOverrideService.GetEffectiveIdentityAsync();

            identity.RealWindowsUsername = UserNameFormatter.StripDomainPrefix(identity.RealWindowsUsername);
            identity.EffectiveWindowsUsername = UserNameFormatter.StripDomainPrefix(identity.EffectiveWindowsUsername);

            return View("User", identity);
        }
    }
}
