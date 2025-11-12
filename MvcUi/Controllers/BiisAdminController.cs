using MvcUi.AuthorizationAttributes;
using MvcUi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SharedServicesLibrary.AppSettings;

namespace MvcUi.Controllers
{
    [AllowRoles("BiisAdmin")]

    public class BiisAdminController : Controller
    {
        private readonly GlobalSettings _globalSettings;
        private readonly UserTrackerService _userTracker;

        public BiisAdminController(IOptions<GlobalSettings> globalSettings,
                                    UserTrackerService userTracker)
        {
            _globalSettings = globalSettings.Value;
            _userTracker = userTracker;
        }


        public IActionResult Index()
        {
            ViewData["AppName"] = _globalSettings.MyGlobalName;

            var users = _userTracker.GetActiveUsers();

            return View("Index", users);
        }
    }
}
