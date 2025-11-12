using DataAccessLibrary.Models;
using SharedViewModelLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServicesLibrary.AdminStaffServices
{
    public class AdminStaffMapper : IAdminStaffMapper
    {
        public AdminStaffMapper()
        {
        }

        public AdminStaffEntityModel ToEntity(AdminStaffViewModel viewModel)
        {
            return new AdminStaffEntityModel
            {
                Id = viewModel.Id,
                WindowsName = (viewModel.WindowsName),
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                FormattedName = viewModel.FormattedName,
                ContactEmail = viewModel.ContactEmail,
                ReceiveEmail = viewModel.ReceiveEmail,
                Active = viewModel.Active
            };
        }

        public AdminStaffViewModel ToViewModel(AdminStaffEntityModel entity)
        {
            return new AdminStaffViewModel
            {
                Id = entity.Id,
                WindowsName = entity.WindowsName,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                // FormattedName taken care of in View model by appending FirstName and LastName
                ContactEmail = entity.ContactEmail,
                ReceiveEmail = entity.ReceiveEmail,
                Active = entity.Active

            };
        }



    }
}
