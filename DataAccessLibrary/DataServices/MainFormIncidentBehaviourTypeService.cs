using DataAccessLibrary.DataAccessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.DataServices
{
    public class MainFormIncidentBehaviourTypeService : IMainFormIncidentBehaviourTypeService
    {
        private readonly IDataAccess _dataAccess;
        private readonly ConnectionStringData _connectionStringData;

        public MainFormIncidentBehaviourTypeService(IDataAccess dataAccess, ConnectionStringData connectionStringData)
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
                    IncidentBehaviourTypeId = sampleCheckboxId
                };

                await _dataAccess.SaveDataAsync<dynamic>(
                            "dbo.spMainFormIncidentBehaviourType_Insert",
                            parameters,
                            _connectionStringData.SqlConnectionName);
            }
        }


        public async Task DeleteByMainFormIdAsync(int mainFormId)
        {
            var parameters = new { MainFormId = mainFormId };
            await _dataAccess.SaveDataAsync<dynamic>(
                    "dbo.spMainFormIncidentBehaviourType_DeleteByMainFormId",
                    parameters,
                    _connectionStringData.SqlConnectionName);
        }


        public async Task<List<int>> GetSelectedIncidentBehaviourTypeIdsByMainFormIdAsync(int mainFormId)
        {
            var parameters = new { MainFormId = mainFormId };

            return await _dataAccess.LoadDataAsync<int, dynamic>(
                                "dbo.spMainFormIncidentBehaviourType_GetIncidentBehaviourTypeIdByMainFormId",
                                parameters,
                                _connectionStringData.SqlConnectionName);
        }




    }
}
