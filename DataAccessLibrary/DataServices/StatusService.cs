using Dapper;
using DataAccessLibrary.DataAccessObjects;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.DataServices
{
    public class StatusService : IStatusService
    {
        private readonly IDataAccess _dataAccess;
        private readonly ConnectionStringData _connectionStringData;
        private readonly string _submittedTitle;

        public StatusService(IDataAccess dataAccess, ConnectionStringData connectionStringData, string submittedTitle)
        {
            _dataAccess = dataAccess;
            _connectionStringData = connectionStringData;
            _submittedTitle = submittedTitle;
        }


        public async Task<int> CreateAsync(StatusEntityModel statusEntityModel)
        {
            // Prepare parameters for stored procedure
            DynamicParameters p = new DynamicParameters();
            p.Add("Title", statusEntityModel.Title);
            p.Add("Active", statusEntityModel.Active);
            p.Add("Order", statusEntityModel.Order);

            p.Add("Id", DbType.Int32, direction: ParameterDirection.Output);

            // Insert record and get the generated Id
            await _dataAccess.SaveDataAsync<dynamic>("dbo.spStatus_Insert", p, _connectionStringData.SqlConnectionName);
            int returnedId = p.Get<int>("Id");

            return returnedId;
        }


        public async Task<int> UpdateAsync(StatusEntityModel statusEntityModel)
        {
            // Update the submission record
            await _dataAccess.SaveDataAsync<dynamic>(
                "dbo.spStatus_Update",
                new
                {
                    Id = statusEntityModel.Id,
                    Title = statusEntityModel.Title,
                    Active = statusEntityModel.Active,
                    Order = statusEntityModel.Order

                },
                _connectionStringData.SqlConnectionName);

            return statusEntityModel.Id;
        }


        public async Task<List<SelectListItem>> GetStatusAllSelectListAsync()
        {
            var statuses = await _dataAccess.LoadDataAsync<StatusEntityModel, dynamic>(
                            "dbo.spStatus_GetAll",
                            new { },
                            _connectionStringData.SqlConnectionName);

            return statuses.Select(f => new SelectListItem
            {
                Value = f.Id.ToString(),
                Text = f.Title
            }).ToList();
        }


        public async Task<List<SelectListItem>> GetStatusAllActiveSelectListAsync()
        {
            var statuses = await _dataAccess.LoadDataAsync<StatusEntityModel, dynamic>(
                            "dbo.spStatus_GetAllActive",
                            new { },
                            _connectionStringData.SqlConnectionName);

            return statuses.Select(f => new SelectListItem
            {
                Value = f.Id.ToString(),
                Text = f.Title
            }).ToList();
        }



        public async Task<string> GetStatusTitleById(int statusId)
        {
            var parameters = new { Id = statusId };

            var rows = await _dataAccess.LoadDataAsync<string, dynamic>(
                            "dbo.spStatus_GetTitleById",
                            parameters,
                            _connectionStringData.SqlConnectionName);

            // Return the Title if found; otherwise, return null or a default message
            return rows.FirstOrDefault();
        }


        public async Task<int?> GetStatusIdByTitle(string statusTitle)
        {
            var parameters = new { Title = statusTitle };

            var rows = await _dataAccess.LoadDataAsync<int, dynamic>(
                            "dbo.spStatus_GetIdByTitle",
                            parameters,
                            _connectionStringData.SqlConnectionName);

            // Return the Id if found; otherwise, return null or a default message
            return rows.FirstOrDefault();
        }


        public async Task<int> GetSubmittedStatusIdAsync()
        {
            // Retrieve the StatusId where Title = 'Open'
            var status = await GetStatusIdByTitle(_submittedTitle);
            return status ?? 0;  // If not found,  return 0
        }





    }
}
