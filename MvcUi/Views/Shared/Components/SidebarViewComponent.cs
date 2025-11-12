using AyrshireCollege.Biis.UserIdentityLibrary.IdentityServices;
using MvcUi.Models;
using MvcUi.Services.ApiClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SharedServicesLibrary.AppSettings;
using SharedServicesLibrary.FormIconServices;
using SharedServicesLibrary.SharedServices.UserRoleServices;
using SharedServicesLibrary.StudentExperienceFormServices;

namespace MvcUi.Views.Shared.Components
{
    public class SidebarViewComponent : ViewComponent
    {
        private readonly IUserRoleService _userRoleService;
        private readonly StudentExperienceFormApiClient _apiClient;
        private readonly IStudentExperienceFormManagementService _studentExperienceFormManagementService;
        private readonly IFormIconService _formIconService;
        private readonly IConfiguration _configuration;
        private readonly GlobalSettings _globalSettings;

        public SidebarViewComponent(IUserRoleService userRoleService,
                                    StudentExperienceFormApiClient apiClient,
                                    IOptions<GlobalSettings> globalSettings)
        {
            _userRoleService = userRoleService;
            _apiClient = apiClient;
            _globalSettings = globalSettings.Value;
        }


        /// <summary>
        /// Invoked when the Sidebar view component is rendered.
        /// Fetches the current user's role and a list of active student experience forms,
        /// then passes them to the view as a SidebarViewModel.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing the rendered view.</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Retrieve the role of the currently logged-in user (e.g., "Admin", "BiisAdmin")
            var userRole = await _userRoleService.GetUserRoleAsync();

            // Retrieve the list of active student experience forms from the database
            // These will be displayed as dynamic links in the sidebar
            var activeFormList = await _apiClient.GetActiveFormsAsync();

            // Populate the SidebarViewModel with the retrieved role and form data
            var model = new SidebarViewModel
            {
                UserRole = userRole,
                FormName = _globalSettings.MyGlobalName,
                Forms = activeFormList
            };

            // Return the view with the model.
            // ASP.NET Core will render: Views/Shared/Components/Sidebar/Default.cshtml
            return View(model);
        }




    }
}
