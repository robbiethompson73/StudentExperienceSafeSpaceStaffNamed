using AyrshireCollege.Biis.UserIdentityLibrary.IdentityServices;
using MvcUi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SharedServicesLibrary.AppSettings;
using SharedServicesLibrary.FormIconServices;
using SharedServicesLibrary.SharedServices.UserRoleServices;
using SharedServicesLibrary.StudentExperienceFormServices;

namespace MvcUi.Views.Shared.Components
{
    public class TopbarViewComponent : ViewComponent
    {
        private readonly IUserRoleService _userRoleService;
        private readonly GlobalSettings _globalSettings;
        private readonly IStudentExperienceFormManagementService _studentExperienceFormManagementService;
        private readonly IFormIconService _formIconServices;

        public TopbarViewComponent(IUserRoleService userRoleService,
                                    IOptions<GlobalSettings> globalSettings)
        {
            _userRoleService = userRoleService;
            _globalSettings = globalSettings.Value;
        }


        /// <summary>
        /// Invoked when the Topbar view component is rendered.
        /// Fetches the current user's role
        /// then passes them to the view as a TopbarViewModel.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing the rendered view.</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Retrieve the role of the currently logged-in user (e.g., "Admin", "BiisAdmin")
            var userRole = await _userRoleService.GetUserRoleAsync();

            // Populate the SidebarViewModel with the retrieved role and form data
            var model = new TopbarViewModel
            {
                UserRole = userRole
            };

            ViewData["AppName"] = _globalSettings.MyGlobalName;

            // Return the view with the model.
            // ASP.NET Core will render: Views/Shared/Components/Sidebar/Default.cshtml
            return View(model);
        }




    }
}
