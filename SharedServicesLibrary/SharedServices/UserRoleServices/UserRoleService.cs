using DataAccessLibrary.DataServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AyrshireCollege.Biis.UserIdentityLibrary.IdentityServices;

namespace SharedServicesLibrary.SharedServices.UserRoleServices
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IIdentityService _identityService;
        private readonly IAdminStaffService _adminStaffService;
        private readonly IBiisAdminStaffService _biisAdminStaffService;

        public UserRoleService(IIdentityService identityService, IBiisAdminStaffService biisAdminStaffService, IAdminStaffService adminStaffService)
        {
            _identityService = identityService;
            _adminStaffService = adminStaffService;
            _biisAdminStaffService = biisAdminStaffService;
        }

        public async Task<string> GetUserRoleAsync()
        {
            var windowsName = await _identityService.GetUnformattedUserNameAsync();

            if (await _biisAdminStaffService.IsBiisStaffAdminByWindowsNameAsync(windowsName))
                return "BiisAdmin";

            if (await _adminStaffService.IsStaffAdminByWindowsNameAsync(windowsName))
                return "Admin";

            if (await _identityService.IsStaffAsync())
                return "Staff";

            if (await _identityService.IsStudentAsync())
                return "Student";

            return "Unknown";
        }


    }
}
