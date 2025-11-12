using DataAccessLibrary.DataAccessObjects;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.DataServices
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IDataAccess _dataAccess;
        private readonly ConnectionStringData _connectionStringData;

        public AuditLogService(IDataAccess dataAccess, ConnectionStringData connectionStringData)
        {
            _dataAccess = dataAccess;
            _connectionStringData = connectionStringData;
        }

        public async Task<List<MainFormAuditLogEntityModel>> GetAuditLogsByMainFormIdAsync(int mainFormId)
        {
            return await _dataAccess.LoadDataAsync<MainFormAuditLogEntityModel, dynamic>(
                                "dbo.spAuditLog_GetByMainFormId",
                                new { MainFormId = mainFormId },
                                _connectionStringData.SqlConnectionName);

        }


    }
}
