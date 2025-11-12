using SharedViewModelLibrary.Models;

namespace SharedServicesLibrary.AdminStaffServices
{
    public interface IAdminStaffManagementService
    {
        Task<int> CreateAdminStaffAsync(AdminStaffViewModel adminStaffViewModel);
        Task<List<AdminStaffViewModel>> GetAdminStaffAllAsync();
        Task<AdminStaffViewModel> GetAdminStaffByIdAsync(int adminStaffId);
        Task<int> UpdateAdminStaffAsync(AdminStaffViewModel adminStaffViewModel);
    }
}