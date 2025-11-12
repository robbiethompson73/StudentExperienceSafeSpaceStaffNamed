using Dapper;
using DataAccessLibrary.DataAccessObjects;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.DataServices
{
    public class AdminStaffService : IAdminStaffService
    {
        private readonly IDataAccess _dataAccess;
        private readonly ConnectionStringData _connectionStringData;

        public AdminStaffService(IDataAccess dataAccess, ConnectionStringData connectionStringData)
        {
            _dataAccess = dataAccess;
            _connectionStringData = connectionStringData;
        }



        public Task<List<AdminStaffEntityModel>> GetAdminStaffAll()
        {
            return _dataAccess.LoadDataAsync<AdminStaffEntityModel, dynamic>(
                                "dbo.spAdminStaff_GetAll",
                                new { },
                                _connectionStringData.SqlConnectionName);
        }

        public Task<List<AdminStaffEntityModel>> GetAdminStaffActive()
        {
            return _dataAccess.LoadDataAsync<AdminStaffEntityModel, dynamic>(
                                "dbo.spAdminStaff_GetAllActive",
                                new { },
                                _connectionStringData.SqlConnectionName);

        }

        public Task<List<string>> GetAdminStaffActiveEmails()
        {
            return _dataAccess.LoadDataAsync<string, dynamic>(
                                "dbo.spAdminStaff_GetAllActiveEmails",
                                new { },
                                _connectionStringData.SqlConnectionName);
        }



        public async Task<AdminStaffEntityModel> GetAdminStaffById(int adminStaffid)
        {

            var rows = await _dataAccess.LoadDataAsync<AdminStaffEntityModel, dynamic>(
                                        "dbo.spAdminStaff_GetById",
                                        new
                                        {
                                            Id = adminStaffid
                                        },
                                        _connectionStringData.SqlConnectionName);

            // rows is a Task of List<> of type AdminStaffEntityModel 
            // return a single AdminStaffEntityModel
            return rows.FirstOrDefault();
        }


        public async Task<bool> IsStaffAdminByWindowsNameAsync(string windowsName)
        {
            var parameters = new { WindowsName = windowsName };

            var result = await _dataAccess.LoadDataAsync<bool, dynamic>(
                                                            "spAdminStaff_IsAdmin_ByWindowsName",
                                                            parameters,
                                                            _connectionStringData.SqlConnectionName);

            // Since the stored procedure always returns 1 or 0, take the first value.
            return result.FirstOrDefault();
        }



        public async Task<string> GetFormattedNameByWindowsNameAsync(string windowsName)
        {
            var parameters = new { WindowsName = windowsName };

            var formattedNameList = await _dataAccess.LoadDataAsync<string, dynamic>(
                                                            "spAdminStaff_GetFormattedNameByWindowsName",
                                                            parameters,
                                                            _connectionStringData.SqlConnectionName);

            // rows is a string 
            return formattedNameList.FirstOrDefault();
        }





        public async Task<int> CreateAsync(AdminStaffEntityModel adminStaffEntityModel)
        {

            // Prepare parameters for stored procedure
            DynamicParameters p = new DynamicParameters();
            p.Add("WindowsName", adminStaffEntityModel.WindowsName);
            p.Add("FirstName", adminStaffEntityModel.FirstName);
            p.Add("LastName", adminStaffEntityModel.LastName);
            p.Add("FormattedName", adminStaffEntityModel.FormattedName);
            p.Add("ContactEmail", adminStaffEntityModel.ContactEmail);
            p.Add("ReceiveEmail", adminStaffEntityModel.ReceiveEmail);
            p.Add("Active", adminStaffEntityModel.Active);

            p.Add("Id", DbType.Int32, direction: ParameterDirection.Output);

            // Insert record and get the generated Id
            await _dataAccess.SaveDataAsync<dynamic>("dbo.spAdminStaff_Insert", p, _connectionStringData.SqlConnectionName);
            int returnedId = p.Get<int>("Id");

            return returnedId;
        }


        public async Task<int> UpdateAsync(AdminStaffEntityModel adminStaffEntityModel)
        {
            // Update the submission record
            await _dataAccess.SaveDataAsync<dynamic>(
                "dbo.spAdminStaff_Update",
                new
                {
                    Id = adminStaffEntityModel.Id,
                    WindowsName = adminStaffEntityModel.WindowsName,
                    FirstName = adminStaffEntityModel.FirstName,
                    LastName = adminStaffEntityModel.LastName,
                    FormattedName = adminStaffEntityModel.FormattedName,
                    ContactEmail = adminStaffEntityModel.ContactEmail,
                    ReceiveEmail = adminStaffEntityModel.ReceiveEmail,
                    Active = adminStaffEntityModel.Active
                },
                _connectionStringData.SqlConnectionName);

            return adminStaffEntityModel.Id;
        }






    }
}
