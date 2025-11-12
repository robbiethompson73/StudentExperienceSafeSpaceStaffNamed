using DataAccessLibrary.DataServices;
using MvcUi.AuthorizationAttributes;
using MvcUi.Models;
using MvcUi.Services.ApiClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SharedServicesLibrary.AppSettings;
using SharedServicesLibrary.FormIconServices;
using SharedServicesLibrary.SharedServices.UserRoleServices;
using SharedServicesLibrary.StudentExperienceFormServices;
using System.Diagnostics;

namespace MvcUi.Controllers
{
    public class HomeController : Controller
    {
        private readonly StudentExperienceFormApiClient _apiClient;
        private readonly IFormIconService _formIconService;
        private readonly IUserRoleService _userRoleService;
        private readonly GlobalSettings _globalSettings;

        public HomeController(StudentExperienceFormApiClient apiClient,
                              IFormIconService formIconService,
                              IUserRoleService userRoleService,
                              IOptions<GlobalSettings> globalSettings)
        {
            _apiClient = apiClient;
            _formIconService = formIconService;
            _userRoleService = userRoleService;
            _globalSettings = globalSettings.Value;
        }


        [AllowRoles("BiisAdmin", "Admin", "Staff", "Student")]
        public async Task<IActionResult> Index(int? id)
        {

            var activeFormList = await _apiClient.GetActiveFormsAsync();


            string userRole = await _userRoleService.GetUserRoleAsync();
            ViewBag.UserRole = userRole;

            ViewData["AppName"] = _globalSettings.MyGlobalName;

            if (id.HasValue)
            {
                // Pass a flag or message to the view indicating success
                ViewData["SuccessMessage"] = "Success";
            }

            return View("Index", activeFormList);
        }



        [AllowRoles("BiisAdmin", "Admin", "Staff", "Student", "Unknown")]
        // Action for Access Denied Page
        public IActionResult AccessRestricted()
        {
            return View("AccessRestricted");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
