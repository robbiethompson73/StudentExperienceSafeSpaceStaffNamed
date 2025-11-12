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
    public class IdentityOverrideDataService : IIdentityOverrideDataService
    {
        private readonly IDataAccess _dataAccess;
        private readonly ConnectionStringData _connectionStringData;

        public IdentityOverrideDataService(IDataAccess dataAccess, ConnectionStringData connectionStringData)
        {
            _dataAccess = dataAccess;
            _connectionStringData = connectionStringData;
        }


        public Task<List<IdentityOverrideEntityModel>> GetIdentityOverridesAll()
        {
            return _dataAccess.LoadDataAsync<IdentityOverrideEntityModel, dynamic>(
                                "dbo.spIdentityOverrides_GetAll",
                                new { },
                                _connectionStringData.SqlConnectionName);
        }


        public async Task<IdentityOverrideEntityModel> GetOverrideByRealWindowsUsernameAsync(string realWindowsUsername)
        {

            var rows = await _dataAccess.LoadDataAsync<IdentityOverrideEntityModel, dynamic>(
                                        "dbo.spIdentityOverrides_GetByRealWindowsUsername",
                                        new
                                        {
                                            RealWindowsUsername = realWindowsUsername
                                        },
                                        _connectionStringData.SqlConnectionName);

            // rows is a Task of List<> of type IdentityOverrideEntityModel 
            // return a single IdentityOverrideEntityModel
            return rows.FirstOrDefault();
        }



        public async Task<IdentityOverrideEntityModel> GetOverrideByIdAsync(int id)
        {
            var rows = await _dataAccess.LoadDataAsync<IdentityOverrideEntityModel, dynamic>(
                                        "dbo.spIdentityOverrides_GetById",
                                        new
                                        {
                                            Id = id
                                        },
                                        _connectionStringData.SqlConnectionName);

            // rows is a Task of List<> of type IdentityOverrideEntityModel 
            // return a single IdentityOverrideEntityModel
            return rows.FirstOrDefault();
        }




        public async Task<int?> CreateAsync(IdentityOverrideEntityModel identityOverrideEntityModel)
        {
            // Check if a record with the same RealWindowsUsername and EffectiveWindowsUsername already exists
            var existing = await _dataAccess.LoadDataAsync<IdentityOverrideEntityModel, dynamic>(
                "dbo.spIdentityOverrides_CheckDuplicate",
                new
                {
                    RealWindowsUsername = identityOverrideEntityModel.RealWindowsUsername
                },
                _connectionStringData.SqlConnectionName
            );

            if (existing.Any())
            {
                // A duplicate record exists — throw to be caught in the controller
                throw new InvalidOperationException("An override for this user already exists.");
            }

            // Prepare parameters for the insert stored procedure
            DynamicParameters p = new DynamicParameters();
            p.Add("RealWindowsUsername", identityOverrideEntityModel.RealWindowsUsername);
            p.Add("RealFormattedName", identityOverrideEntityModel.RealFormattedName);
            p.Add("EffectiveWindowsUsername", identityOverrideEntityModel.EffectiveWindowsUsername);
            p.Add("EffectiveFormattedName", identityOverrideEntityModel.EffectiveFormattedName);
            p.Add("Id", DbType.Int32, direction: ParameterDirection.Output);

            // Perform insert and retrieve new ID
            await _dataAccess.SaveDataAsync<dynamic>(
                "dbo.spIdentityOverrides_Insert",
                p,
                _connectionStringData.SqlConnectionName
            );

            int returnedId = p.Get<int>("Id");

            return returnedId;
        }



        public async Task<int?> UpdateAsync(IdentityOverrideEntityModel identityOverrideEntityModel)
        {
            // Update the submission record
            await _dataAccess.SaveDataAsync<dynamic>(
                "dbo.spIdentityOverrides_Update",
                new
                {
                    Id = identityOverrideEntityModel.Id,
                    RealWindowsUsername = identityOverrideEntityModel.RealWindowsUsername,
                    RealFormattedName = identityOverrideEntityModel.RealFormattedName,
                    EffectiveWindowsUsername = identityOverrideEntityModel.EffectiveWindowsUsername,
                    EffectiveFormattedName = identityOverrideEntityModel.EffectiveFormattedName
                },
                _connectionStringData.SqlConnectionName);

            return identityOverrideEntityModel.Id;
        }


        public async Task<int> DeleteById(int id)
        {
            await _dataAccess.SaveDataAsync<dynamic>(
                "dbo.spIdentityOverrides_DeleteById",
                new { Id = id },
                _connectionStringData.SqlConnectionName);

            return id; // or return 1 to indicate success, since SaveDataAsync probably returns void
        }



    }
}
