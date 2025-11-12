using DataAccessLibrary.DataServices;
using DataAccessLibrary.Models;
using MvcUi.AuthorizationAttributes;
using MvcUi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SharedServicesLibrary.AdminStaffServices;
using SharedServicesLibrary.AppSettings;
using SharedViewModelLibrary.Models;

namespace FormToolkitMvc.Controllers
{

    [AllowRoles("BiisAdmin", "Admin")]
    public class AdminStaffController : Controller
    {
        private readonly IAdminStaffManagementService _adminStaffManagementService;
        private readonly GlobalSettings _globalSettings;


        public AdminStaffController(IAdminStaffManagementService adminStaffManagementService,
                                    IOptions<GlobalSettings> globalSettings
                                    )
        {
            _adminStaffManagementService = adminStaffManagementService;
            _globalSettings = globalSettings.Value;

        }



        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var adminStaffViewModel = new AdminStaffViewModel()
            {
                Active = true,
                ReceiveEmail = true// Set default to 'Yes'
            };

            ViewData["AppName"] = _globalSettings.MyGlobalName;

            return View("Create", adminStaffViewModel);
        }



        [HttpPost]
        public async Task<IActionResult> Create(AdminStaffViewModel adminStaffViewModel)
        {

            if (ModelState.IsValid == false)
            {
                return View("Create", adminStaffViewModel);
            }

            int newId = await _adminStaffManagementService.CreateAdminStaffAsync(adminStaffViewModel);

            // Store a success message in TempData to persist it across the redirect
            TempData["SuccessMessage"] = "Success";

            return RedirectToAction("Index", "Admin", new { Id = newId });
        }



        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var adminStaffViewModel = await _adminStaffManagementService.GetAdminStaffByIdAsync(id);

            ViewData["AppName"] = _globalSettings.MyGlobalName;

            return View("Update", adminStaffViewModel);
        }



        [HttpPost]
        public async Task<IActionResult> Update(AdminStaffViewModel adminStaffViewModel)
        {
            if (ModelState.IsValid == false)
            {
                return View("Create", adminStaffViewModel);
            }

            int newId = await _adminStaffManagementService.UpdateAdminStaffAsync(adminStaffViewModel);

            // Store a success message in TempData to persist it across the redirect
            TempData["SuccessMessage"] = "Success";

            return RedirectToAction("Details", "AdminStaff", new { Id = newId });
        }






        public async Task<IActionResult> Details(int id)
        {
            var adminStaffViewModel = await _adminStaffManagementService.GetAdminStaffByIdAsync(id);

            ViewData["AppName"] = _globalSettings.MyGlobalName;

            return View("Details", adminStaffViewModel);
        }





    }
}
