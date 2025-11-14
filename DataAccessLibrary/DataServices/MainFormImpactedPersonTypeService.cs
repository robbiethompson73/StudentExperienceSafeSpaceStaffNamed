using DataAccessLibrary.DataAccessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.DataServices
{
    public class MainFormImpactedPersonTypeService : IMainFormImpactedPersonTypeService
    {
        private readonly IDataAccess _dataAccess;
        private readonly ConnectionStringData _connectionStringData;

        public MainFormImpactedPersonTypeService(IDataAccess dataAccess, ConnectionStringData connectionStringData)
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
                    ImpactedPersonTypeId = sampleCheckboxId
                };

                await _dataAccess.SaveDataAsync<dynamic>(
                            "dbo.spMainFormImpactedPersonType_Insert",
                            parameters,
                            _connectionStringData.SqlConnectionName);
            }
        }


        public async Task DeleteByMainFormIdAsync(int mainFormId)
        {
            var parameters = new { MainFormId = mainFormId };
            await _dataAccess.SaveDataAsync<dynamic>(
                    "dbo.spMainFormImpactedPersonType_DeleteByMainFormId",
                    parameters,
                    _connectionStringData.SqlConnectionName);
        }


        public async Task<List<int>> GetSelectedImpactedPersonTypeIdsByMainFormIdAsync(int mainFormId)
        {
            var parameters = new { MainFormId = mainFormId };

            return await _dataAccess.LoadDataAsync<int, dynamic>(
                                "dbo.spMainFormImpactedPersonType_GetImpactedPersonTypeIdByMainFormId",
                                parameters,
                                _connectionStringData.SqlConnectionName);
        }




    }
}
