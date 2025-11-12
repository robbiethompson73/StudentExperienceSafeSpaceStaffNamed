using MvcUi.AuthorizationAttributes;
using MvcUi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SharedServicesLibrary.AppSettings;

namespace MvcUi.Controllers
{
    public class ContactController : Controller
    {
        private readonly GlobalSettings _globalSettings;
        public ContactController(IOptions<GlobalSettings> globalSettings)
        {
            _globalSettings = globalSettings.Value;
        }


        [AllowRoles("BiisAdmin", "Admin", "Staff", "Student")]
        public IActionResult Index()
        {

            ViewData["AppName"] = _globalSettings.MyGlobalName;

            return View("Index");
        }
    }
}
