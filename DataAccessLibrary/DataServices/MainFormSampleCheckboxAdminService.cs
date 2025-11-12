using DataAccessLibrary.DataAccessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.DataServices
{
    public class MainFormSampleCheckboxAdminService : IMainFormSampleCheckboxAdminService
    {
        private readonly IDataAccess _dataAccess;
        private readonly ConnectionStringData _connectionStringData;

        public MainFormSampleCheckboxAdminService(IDataAccess dataAccess, ConnectionStringData connectionStringData)
        {
            _dataAccess = dataAccess;
            _connectionStringData = connectionStringData;
        }

        public async Task CreateAndDeleteAsync(int mainFormId, List<int> checkboxIds)
        {
            // First, delete existing records for the mainFormId
            await DeleteByMainFormIdAsync(mainFormId);

            // Now insert the new selections from the checkbox list
            await CreateAsync(mainFormId, checkboxIds);
        }


        // Insert records into bridging table for a given MainFormId
        public async Task CreateAsync(int mainFormId, List<int> checkboxIds)
        {
            if (checkboxIds == null || !checkboxIds.Any())
            {
                // Nothing to insert — exit early
                return;
            }

            foreach (var checkboxId in checkboxIds)
            {
                var parameters = new
                {
                    MainFormId = mainFormId,
                    SampleCheckboxAdminId = checkboxId
                };

                await _dataAccess.SaveDataAsync<dynamic>(
                            "dbo.spMainFormSampleCheckboxAdmin_Insert",
                            parameters,
                            _connectionStringData.SqlConnectionName);
            }
        }


        public async Task DeleteByMainFormIdAsync(int mainFormId)
        {
            var parameters = new { MainFormId = mainFormId };
            await _dataAccess.SaveDataAsync<dynamic>(
                    "dbo.spMainFormSampleCheckboxAdmin_DeleteByMainFormId",
                    parameters,
                    _connectionStringData.SqlConnectionName);
        }


        public async Task<List<int>> GetSelectedSampleCheckboxAdminIdsByMainFormIdAsync(int mainFormId)
        {
            var parameters = new { MainFormId = mainFormId };

            return await _dataAccess.LoadDataAsync<int, dynamic>(
                                "dbo.spMainFormSampleCheckboxAdmin_GetSampleCheckboxAdminIdByMainFormId",
                                parameters,
                                _connectionStringData.SqlConnectionName);
        }




    }
}
