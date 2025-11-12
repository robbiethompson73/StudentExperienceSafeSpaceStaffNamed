using DataAccessLibrary.DataAccessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.DataServices
{
    public class MainFormSampleCheckboxService : IMainFormSampleCheckboxService
    {
        private readonly IDataAccess _dataAccess;
        private readonly ConnectionStringData _connectionStringData;

        public MainFormSampleCheckboxService(IDataAccess dataAccess, ConnectionStringData connectionStringData)
        {
            _dataAccess = dataAccess;
            _connectionStringData = connectionStringData;
        }

        public async Task CreateAndDeleteAsync(int mainFormId, List<int> sampleCheckboxIds)
        {
            // First, delete existing records for the mainFormId
            await DeleteByMainFormIdAsync(mainFormId);

            // Now insert the new selections from the checkbox list
            await CreateAsync(mainFormId, sampleCheckboxIds);
        }


        // Insert records into bridging table for a given MainFormId
        public async Task CreateAsync(int mainFormId, List<int> sampleCheckboxIds)
        {
            if (sampleCheckboxIds == null || !sampleCheckboxIds.Any())
            {
                // Nothing to insert — exit early
                return;
            }

            foreach (var sampleCheckboxId in sampleCheckboxIds)
            {
                var parameters = new
                {
                    MainFormId = mainFormId,
                    SampleCheckboxId = sampleCheckboxId
                };

                await _dataAccess.SaveDataAsync<dynamic>(
                            "dbo.spMainFormSampleCheckbox_Insert",
                            parameters,
                            _connectionStringData.SqlConnectionName);
            }
        }


        public async Task DeleteByMainFormIdAsync(int mainFormId)
        {
            var parameters = new { MainFormId = mainFormId };
            await _dataAccess.SaveDataAsync<dynamic>(
                    "dbo.spMainFormSampleCheckbox_DeleteByMainFormId",
                    parameters,
                    _connectionStringData.SqlConnectionName);
        }


        public async Task<List<int>> GetSelectedSampleCheckboxIdsByMainFormIdAsync(int mainFormId)
        {
            var parameters = new { MainFormId = mainFormId };

            return await _dataAccess.LoadDataAsync<int, dynamic>(
                                "dbo.spMainFormSampleCheckbox_GetSampleCheckboxIdByMainFormId",
                                parameters,
                                _connectionStringData.SqlConnectionName);
        }




    }
}
