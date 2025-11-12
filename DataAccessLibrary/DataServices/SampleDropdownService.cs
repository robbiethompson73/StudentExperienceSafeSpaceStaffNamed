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
    public class SampleDropdownService : ISampleDropdownService
    {

        private readonly IDataAccess _dataAccess;
        private readonly ConnectionStringData _connectionStringData;

        public SampleDropdownService(IDataAccess dataAccess, ConnectionStringData connectionStringData)
        {
            _dataAccess = dataAccess;
            _connectionStringData = connectionStringData;
        }


        public async Task<string> GetTitleById(int? id)
        {
            var parameters = new { Id = id };

            var rows = await _dataAccess.LoadDataAsync<string, dynamic>(
                "dbo.spSampleDropdown_GetTitleById",
                parameters,
                _connectionStringData.SqlConnectionName);

            // Return the Title if found; otherwise, return null or a default message
            return rows.FirstOrDefault();
        }





        public async Task<List<SelectListItem>> GetAllActiveSelectListAsync()
        {
            var statuses = await _dataAccess.LoadDataAsync<SampleDropdownEntityModel, dynamic>(
                            "dbo.spSampleDropdown_GetAllActive",
                            new { },
                            _connectionStringData.SqlConnectionName);

            return statuses.Select(f => new SelectListItem
            {
                Value = f.Id.ToString(),
                Text = f.Title
            }).ToList();
        }




    }
}
