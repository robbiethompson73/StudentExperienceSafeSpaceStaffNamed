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
    public class StudentExperienceFormService : IStudentExperienceFormService
    {
        private readonly IDataAccess _dataAccess;
        private readonly ConnectionStringData _connectionStringData;

        public StudentExperienceFormService(IDataAccess dataAccess, ConnectionStringData connectionStringData)
        {
            _dataAccess = dataAccess;
            _connectionStringData = connectionStringData;
        }


        public Task<List<StudentExperienceFormEntityModel>> GetStudentExperienceFormActive()
        {
            return _dataAccess.LoadDataAsync<StudentExperienceFormEntityModel, dynamic>(
                                "dbo.spStudentExperienceForm_GetAllActive",
                                new { },
                                _connectionStringData.SqlConnectionName);
        }





    }
}
