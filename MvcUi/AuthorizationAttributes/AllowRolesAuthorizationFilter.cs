using AyrshireCollege.Biis.UserIdentityLibrary.IdentityServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.VisualBasic;
using SharedServicesLibrary.SharedServices.UserRoleServices;
using System.Net.NetworkInformation;
using System;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MvcUi.AuthorizationAttributes
{
    public class AllowRolesAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly IUserRoleService _userRoleService;
        private readonly IIdentityService _identityService;
        
        private readonly string[] _roles; // Array stored roles passed from AllowRoles attribute e.g. [AllowRoles("BiisAdmin","Admin")]

        public AllowRolesAuthorizationFilter(IUserRoleService userRoleService, IIdentityService identityService, string[] roles)
        {
            _userRoleService = userRoleService;
            _identityService = identityService;
            _roles = roles;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Argument 
            //  AuthorizationFilterContext provides all the information needed to:
            //	Access the HTTP request and user info,
            //	Determine which controller / action is being accessed,
            //	And control the outcome of the authorization(e.g., allow or deny access).

            // Retrieve the user role using the identity name
            string userRole = await _userRoleService.GetUserRoleAsync();

            // Check if the user's role is in the allowed roles
            bool hasAccess = _roles.Contains(userRole); // check if user's role is found within array from AllowRoles attribute e.g. [AllowRoles("BiisAdmin","Admin")]

            if (hasAccess == false)
            {
                // Deny access if the user doesn't have any of the allowed roles
                // Redirect to HomeController.AccessRestricted action
                context.Result = new RedirectToActionResult("AccessRestricted", "Home", null);
            }
        }

    }




}
