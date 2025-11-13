using DataAccessLibrary.DataAccessObjects;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.DataServices
{
    public class IncidentLocationService : IIncidentLocationService
    {
        private readonly IDataAccess _dataAccess;
        private readonly ConnectionStringData _connectionStringData;

        public IncidentLocationService(IDataAccess dataAccess, ConnectionStringData connectionStringData)
        {
            _dataAccess = dataAccess;
            _connectionStringData = connectionStringData;
        }


        public Task<List<IncidentLocationEntityModel>> GetAllActive()
        {
            return _dataAccess.LoadDataAsync<IncidentLocationEntityModel, dynamic>(
                                "dbo.spIncidentLocation_GetAllActive",
                                new { },
                                _connectionStringData.SqlConnectionName);
        }

        public async Task<List<SelectListItem>> GetAllActiveSelectListAsync()
        {
            var radioEntities = await GetAllActive();

            return radioEntities.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Title
            }).ToList();
        }


        public async Task<IncidentLocationEntityModel> GetIncidentLocationById(int id)
        {

            var rows = await _dataAccess.LoadDataAsync<IncidentLocationEntityModel, dynamic>(
                                        "dbo.spIncidentLocation_GetById",
                                        new
                                        {
                                            Id = id
                                        },
                                        _connectionStringData.SqlConnectionName);

            // rows is a Task of List<> of type AdminStaffEntityModel 
            // return a single AdminStaffEntityModel
            return rows.FirstOrDefault();
        }


        public async Task<string> GetTitleById(int id)
        {
            var parameters = new { Id = id };

            var rows = await _dataAccess.LoadDataAsync<string, dynamic>(
                            "dbo.spIncidentLocation_GetTitleById",
                            parameters,
                            _connectionStringData.SqlConnectionName);

            // Return the Id if found; otherwise, return null or a default message
            return rows.FirstOrDefault();
        }





    }
}
