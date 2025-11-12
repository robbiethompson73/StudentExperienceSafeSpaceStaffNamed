using DataAccessLibrary.DataAccessObjects;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.DataServices
{
    public class BiisAdminStaffService : IBiisAdminStaffService
    {
        private readonly IDataAccess _dataAccess;
        private readonly ConnectionStringData _connectionStringData;

        public BiisAdminStaffService(IDataAccess dataAccess, ConnectionStringData connectionStringData)
        {
            _dataAccess = dataAccess;
            _connectionStringData = connectionStringData;
        }


        public Task<List<BiisAdminStaffEntityModel>> GetBiisAdminStaffActive()
        {
            return _dataAccess.LoadDataAsync<BiisAdminStaffEntityModel, dynamic>(
                                "dbo.spBiisAdminStaff_GetAllActive",
                                new { },
                                _connectionStringData.SqlConnectionName);

        }

        public Task<List<string>> GetBiisAdminStaffActiveEmails()
        {
            return _dataAccess.LoadDataAsync<string, dynamic>(
                                "dbo.spBiisAdminStaff_GetAllActiveEmails",
                                new { },
                                _connectionStringData.SqlConnectionName);
        }

        public Task<List<BiisAdminStaffEntityModel>> GetBiisAdminStaffAll()
        {
            return _dataAccess.LoadDataAsync<BiisAdminStaffEntityModel, dynamic>(
                                "dbo.spBiisAdminStaff_GetAll",
                                new { },
                                _connectionStringData.SqlConnectionName);

        }



        public async Task<bool> IsBiisStaffAdminByWindowsNameAsync(string windowsName)
        {
            var parameters = new { WindowsName = windowsName };

            var result = await _dataAccess.LoadDataAsync<bool, dynamic>(
                                                            "spBiisAdminStaff_IsAdmin_ByWindowsName",
                                                            parameters,
                                                            _connectionStringData.SqlConnectionName);

            // Since the stored procedure always returns 1 or 0, take the first value.
            return result.FirstOrDefault();
        }





    }
}
