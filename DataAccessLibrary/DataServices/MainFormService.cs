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
    public class MainFormService : IMainFormService
    {
        private readonly IDataAccess _dataAccess;
        private readonly IMainFormSampleCheckboxService _mainFormSampleCheckboxService;
        private readonly IStatusService _statusService;
        private readonly ConnectionStringData _connectionStringData;

        public MainFormService(IDataAccess dataAccess,
                               ConnectionStringData connectionStringData,
                               IStatusService statusService,
                               IMainFormSampleCheckboxService mainFormSampleCheckboxService
                               )
        {
            _dataAccess = dataAccess;
            _mainFormSampleCheckboxService = mainFormSampleCheckboxService;
            _statusService = statusService;
            _connectionStringData = connectionStringData;
        }

        public async Task<int> CreateAsync(MainFormEntityModel mainFormEntityModel,
                                            List<MainFormAuditLogEntityModel> auditEntries)
        {
            // Prepare parameters for stored procedure
            DynamicParameters p = new DynamicParameters();
            p.Add("StudentReferenceNumber", mainFormEntityModel.StudentReferenceNumber);
            p.Add("StudentDateOfBirth", mainFormEntityModel.StudentDateOfBirth);
            p.Add("SubmittedByWindowsUserName", mainFormEntityModel.SubmittedByWindowsUserName);

            // Textboxes
            p.Add("StudentFullName", mainFormEntityModel.StudentFullName);
            p.Add("StaffFullName", mainFormEntityModel.StaffFullName);
            p.Add("StaffTelephoneNumber", mainFormEntityModel.StaffTelephoneNumber);
            p.Add("StaffEmail", mainFormEntityModel.StaffEmail);
            p.Add("IncidentPersonName", mainFormEntityModel.IncidentPersonName);
            p.Add("IncidentDate", mainFormEntityModel.IncidentDate);

            p.Add("SampleDate", mainFormEntityModel.SampleDate);
            p.Add("SampleTime", mainFormEntityModel.SampleTime);
            p.Add("SampleCost", mainFormEntityModel.SampleCost);

            p.Add("SampleTextbox", mainFormEntityModel.SampleTextbox);


            // Textareas
            p.Add("SampleTextarea", mainFormEntityModel.SampleTextarea);


            // DropdownLists
            p.Add("SampleDropDownId", mainFormEntityModel.SampleDropdownId);


            // Radios
            p.Add("IncidentHappenedToId", mainFormEntityModel.IncidentHappenedToId);
            p.Add("NumberOfPeopleImpactedId", mainFormEntityModel.NumberOfPeopleImpactedId);
            p.Add("NumberOfPeopleCausedIncidentId", mainFormEntityModel.NumberOfPeopleCausedIncidentId);
            p.Add("IncidentLocationId", mainFormEntityModel.IncidentLocationId);
            p.Add("HasSimilarIncidentHappenedBeforeId", mainFormEntityModel.HasSimilarIncidentHappenedBeforeId);
            p.Add("SampleRadioId", mainFormEntityModel.SampleRadioId);


            // Checkboxes
            // Requires insert into bridging tables - see below


            // StatusId has default value of 1 (Open)

            p.Add("Id", DbType.Int32, direction: ParameterDirection.Output);

            // Insert the submission record and get the generated Id
            await _dataAccess.SaveDataAsync<dynamic>("dbo.spMainForm_Insert", p, _connectionStringData.SqlConnectionName);
            int submissionId = p.Get<int>("Id");

            // Checkbox Bridging Tables

            // Insert records into bridging table tblMainFormSampleCheckbox
            await _mainFormSampleCheckboxService.CreateAsync(submissionId, mainFormEntityModel.SelectedSampleCheckboxIds);


            // Insert selected Status into bridging table tblMainFormStatus
            int statusId = await _statusService.GetSubmittedStatusIdAsync();


            // Save audit logs if any
            if (auditEntries != null && auditEntries.Count > 0)
            {
                foreach (var entry in auditEntries)
                {
                    entry.MainFormId = submissionId;
                    await _dataAccess.SaveDataAsync("dbo.spAuditLog_Insert", entry, _connectionStringData.SqlConnectionName);
                }
            }

            return submissionId;
        }




        public Task<List<MainFormEntityModel>> GetAll()
        {
            return _dataAccess.LoadDataAsync<MainFormEntityModel, dynamic>(
                                "dbo.spMainForm_GetAll",
                                new { },
                                _connectionStringData.SqlConnectionName);
        }


        public async Task<List<MainFormEntityModel>> GetBySubmittedByWindowsUserName(string windowsName)
        {
            return await _dataAccess.LoadDataAsync<MainFormEntityModel, dynamic>(
                                        "dbo.spMainForm_GetBySubmittedByWindowsUserName",
                                        new
                                        {
                                            SubmittedByWindowsUserName = windowsName
                                        },
                                        _connectionStringData.SqlConnectionName);
        }



        public async Task<MainFormEntityModel> GetById(int submissionId)
        {
            var rows = await _dataAccess.LoadDataAsync<MainFormEntityModel, dynamic>(
                                        "dbo.spMainForm_GetById",
                                        new
                                        {
                                            Id = submissionId
                                        },
                                        _connectionStringData.SqlConnectionName);

            // rows is a Task of List<> of type BiisStaffModel 
            // return a single BiisStaffModel
            return rows.FirstOrDefault();
        }


        public async Task<int> GetSampleRadioIdByMainFormId(int submissionId)
        {
            var parameters = new { Id = submissionId };

            var rows = await _dataAccess.LoadDataAsync<int, dynamic>(
                                        "dbo.spMainForm_GetSampleRadioIdByMainFormId",
                                        parameters,
                                        _connectionStringData.SqlConnectionName);
            // rows is a Task of List<> of type BiisStaffModel 
            // return a single BiisStaffModel
            return rows.FirstOrDefault();
        }


        public async Task<int> GetIncidentHappenedToIdByMainFormId(int submissionId)
        {
            var parameters = new { Id = submissionId };

            var rows = await _dataAccess.LoadDataAsync<int, dynamic>(
                                        "dbo.spMainForm_GetIncidentHappenedToIdByMainFormId",
                                        parameters,
                                        _connectionStringData.SqlConnectionName);
            // rows is a Task of List<> of type BiisStaffModel 
            // return a single BiisStaffModel
            return rows.FirstOrDefault();
        }


        public async Task<int> GetNumberOfPeopleImpactedIdByMainFormId(int submissionId)
        {
            var parameters = new { Id = submissionId };

            var rows = await _dataAccess.LoadDataAsync<int, dynamic>(
                                        "dbo.spMainForm_NumberOfPeopleImpactedIdByMainFormId",
                                        parameters,
                                        _connectionStringData.SqlConnectionName);
            // rows is a Task of List<> of type BiisStaffModel 
            // return a single BiisStaffModel
            return rows.FirstOrDefault();
        }



        public async Task<int> GetNumberOfPeopleCausedIncidentIdByMainFormId(int submissionId)
        {
            var parameters = new { Id = submissionId };

            var rows = await _dataAccess.LoadDataAsync<int, dynamic>(
                                        "dbo.spMainForm_NumberOfPeopleCausedIncidentIdByMainFormId",
                                        parameters,
                                        _connectionStringData.SqlConnectionName);
            // rows is a Task of List<> of type BiisStaffModel 
            // return a single BiisStaffModel
            return rows.FirstOrDefault();
        }


        public async Task<int> GetIncidentLocationIdByMainFormId(int submissionId)
        {
            var parameters = new { Id = submissionId };

            var rows = await _dataAccess.LoadDataAsync<int, dynamic>(
                                        "dbo.spMainForm_IncidentLocationIdByMainFormId",
                                        parameters,
                                        _connectionStringData.SqlConnectionName);
            // rows is a Task of List<> of type BiisStaffModel 
            // return a single BiisStaffModel
            return rows.FirstOrDefault();
        }


        public async Task<int> GetHasSimilarIncidentHappenedBeforeIdByMainFormId(int submissionId)
        {
            var parameters = new { Id = submissionId };

            var rows = await _dataAccess.LoadDataAsync<int, dynamic>(
                                        "dbo.spMainForm_HasSimilarIncidentHappenedBeforeIdByMainFormId",
                                        parameters,
                                        _connectionStringData.SqlConnectionName);
            // rows is a Task of List<> of type BiisStaffModel 
            // return a single BiisStaffModel
            return rows.FirstOrDefault();
        }
















    }
}
