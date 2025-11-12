using DataAccessLibrary.Models;
using SharedViewModelLibrary.Models;

namespace SharedServicesLibrary.AdminStaffServices
{
    public interface IAdminStaffMapper
    {
        AdminStaffEntityModel ToEntity(AdminStaffViewModel viewModel);
        AdminStaffViewModel ToViewModel(AdminStaffEntityModel entity);
    }
}