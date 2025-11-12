using Dapper;
using DataAccessLibrary.DataAccessObjects;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.DataServices
{
    public class SampleCheckboxService : ISampleCheckboxService
    {

        private readonly IDataAccess _dataAccess;
        private readonly ConnectionStringData _connectionStringData;

        public SampleCheckboxService(IDataAccess dataAccess, ConnectionStringData connectionStringData)
        {
            _dataAccess = dataAccess;
            _connectionStringData = connectionStringData;
        }


        public async Task<int> CreateAsync(SampleCheckboxEntityModel entityModel)
        {
            // Prepare parameters for stored procedure
            DynamicParameters p = new DynamicParameters();
            p.Add("Title", entityModel.Title);
            p.Add("Active", entityModel.Active);
            p.Add("Order", entityModel.Order);

            p.Add("Id", DbType.Int32, direction: ParameterDirection.Output);

            // Insert record and get the generated Id
            await _dataAccess.SaveDataAsync<dynamic>("dbo.spSampleCheckbox_Insert", p, _connectionStringData.SqlConnectionName);
            int returnedId = p.Get<int>("Id");

            return returnedId;
        }


        public async Task<int> UpdateAsync(SampleCheckboxEntityModel entityModel)
        {
            // Update the submission record
            await _dataAccess.SaveDataAsync<dynamic>(
                "dbo.spSampleCheckbox_Update",
                new
                {
                    Id = entityModel.Id,
                    Title = entityModel.Title,
                    Active = entityModel.Active,
                    Order = entityModel.Order

                },
                _connectionStringData.SqlConnectionName);

            return entityModel.Id;
        }


        public async Task<List<SelectListItem>> GetAllSelectListAsync()
        {
            var rows = await _dataAccess.LoadDataAsync<SampleCheckboxEntityModel, dynamic>(
                            "dbo.spSampleCheckbox_GetAll",
                            new { },
                            _connectionStringData.SqlConnectionName);

            return rows.Select(f => new SelectListItem
            {
                Value = f.Id.ToString(),
                Text = f.Title
            }).ToList();
        }


        public async Task<List<SelectListItem>> GetAllActiveSelectListAsync()
        {
            var rows = await _dataAccess.LoadDataAsync<SampleCheckboxEntityModel, dynamic>(
                            "dbo.spSampleCheckbox_GetAllActive",
                            new { },
                            _connectionStringData.SqlConnectionName);

            return rows.Select(f => new SelectListItem
            {
                Value = f.Id.ToString(),
                Text = f.Title
            }).ToList();
        }



        public async Task<string> GetTitleById(int id)
        {
            var parameters = new { Id = id };

            var rows = await _dataAccess.LoadDataAsync<string, dynamic>(
                            "dbo.spSampleCheckbox_GetTitleById",
                            parameters,
                            _connectionStringData.SqlConnectionName);

            // Return the Title if found; otherwise, return null or a default message
            return rows.FirstOrDefault();
        }









    }
}
