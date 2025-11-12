using DataAccessLibrary.Models;

namespace DataAccessLibrary.DataServices
{
    public interface IBiisAdminStaffService
    {
        Task<List<BiisAdminStaffEntityModel>> GetBiisAdminStaffActive();
        Task<List<string>> GetBiisAdminStaffActiveEmails();
        Task<List<BiisAdminStaffEntityModel>> GetBiisAdminStaffAll();
        Task<bool> IsBiisStaffAdminByWindowsNameAsync(string windowsName);
    }
}