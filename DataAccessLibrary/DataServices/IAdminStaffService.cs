using DataAccessLibrary.Models;

namespace DataAccessLibrary.DataServices
{
    public interface IAdminStaffService
    {
        Task<int> CreateAsync(AdminStaffEntityModel adminStaffEntityModel);
        Task<List<AdminStaffEntityModel>> GetAdminStaffActive();
        Task<List<string>> GetAdminStaffActiveEmails();
        Task<List<AdminStaffEntityModel>> GetAdminStaffAll();
        Task<AdminStaffEntityModel> GetAdminStaffById(int adminStaffid);
        Task<string> GetFormattedNameByWindowsNameAsync(string windowsName);
        Task<bool> IsStaffAdminByWindowsNameAsync(string windowsName);
        Task<int> UpdateAsync(AdminStaffEntityModel adminStaffEntityModel);
    }
}