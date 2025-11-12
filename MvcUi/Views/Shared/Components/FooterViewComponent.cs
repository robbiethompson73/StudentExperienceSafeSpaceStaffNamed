using AyrshireCollege.Biis.UserIdentityLibrary.IdentityServices;
using MvcUi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SharedServicesLibrary.FormIconServices;
using SharedServicesLibrary.SharedServices.UserRoleServices;
using SharedServicesLibrary.StudentExperienceFormServices;

namespace MvcUi.Views.Shared.Components
{
    public class FooterViewComponent : ViewComponent
    {


        public FooterViewComponent()
        {
        }


        /// <summary>
        /// Invoked when the Topbar view component is rendered.
        /// Fetches the current user's role
        /// then passes them to the view as a TopbarViewModel.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing the rendered view.</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }




    }
}
