using AyrshireCollege.Biis.UserIdentityLibrary.IdentityServices;
using DataAccessLibrary.DataServices;
using Microsoft.AspNetCore.Mvc;
using SharedServicesLibrary.SharedServices.UserRoleServices;


namespace MvcUi.Views.Shared.Components
{
    public class UserGreetingViewComponent : ViewComponent
    {
        private readonly IIdentityService _identityService;
        private readonly IBiisAdminStaffService _biisAdminStaffServices;
        private readonly IAdminStaffService _adminStaffServices;
        private readonly IUserRoleService _userRoleService;

        public UserGreetingViewComponent(IIdentityService identityService, IBiisAdminStaffService biisAdminStaffServices, IAdminStaffService adminStaffServices, IUserRoleService userRoleService)
        {
            _identityService = identityService;
            _biisAdminStaffServices = biisAdminStaffServices;
            _adminStaffServices = adminStaffServices;
            _userRoleService = userRoleService;
        }

        // Method to fetch the formatted username and pass it to the view
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Get the windows username from the current user
            string windowsUsername = await _identityService.GetUnformattedUserNameAsync();

            // Format the username (i.e., remove domain or whatever is needed)
            string formattedUsername = await _identityService.GetFormattedUserNameAsync();

            // Check if the user is an admin
            bool isBiisAdmin = await _biisAdminStaffServices.IsBiisStaffAdminByWindowsNameAsync(windowsUsername);

            bool isAdmin = await _adminStaffServices.IsStaffAdminByWindowsNameAsync(windowsUsername);

            if (isAdmin == true)
            {
                formattedUsername = await _adminStaffServices.GetFormattedNameByWindowsNameAsync(windowsUsername);
            }

            string role = await _userRoleService.GetUserRoleAsync();

            // Return the formatted username and admin status to the view
            var viewModel = new { FormattedUsername = formattedUsername, Role = role, IsBiisAdmin = isBiisAdmin, IsAdmin = isAdmin };

            return View("Default", viewModel);
        }
    }
}
