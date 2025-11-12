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
    public class ExcelDownloadService : IExcelDownloadService
    {
        private readonly IDataAccess _dataAccess;
        private readonly ConnectionStringData _connectionStringData;

        public ExcelDownloadService(IDataAccess dataAccess,
                                        ConnectionStringData connectionStringData)
        {
            _dataAccess = dataAccess;
            _connectionStringData = connectionStringData;
        }


        public Task<List<ExcelEntityModel>> GetDataForExcelExport()
        {
            return _dataAccess.LoadDataAsync<ExcelEntityModel, dynamic>(
                                "dbo.spMainFormAdmin_ExportToExcel",
                                new { },
                                _connectionStringData.SqlConnectionName);
        }








    }
}
