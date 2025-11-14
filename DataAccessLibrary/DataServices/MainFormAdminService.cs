using Dapper;
using DataAccessLibrary.DataAccessObjects;
using DataAccessLibrary.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataAccessLibrary.DataServices
{
    public class MainFormAdminService : IMainFormAdminService
    {
        private readonly IDataAccess _dataAccess;
        private readonly IMainFormSampleCheckboxService _mainFormSampleCheckboxService;
        private readonly ConnectionStringData _connectionStringData;
        private readonly IConfiguration _configuration;
        private readonly IMainFormSampleCheckboxAdminService _mainFormSampleCheckboxAdminService;
        private readonly IMainFormImpactedPersonTypeService _mainFormImpactedPersonTypeService;
        private readonly IMainFormIncidentBehaviourTypeService _mainFormIncidentBehaviourTypeService;
        private readonly IMainFormIncidentMotivationTypeService _mainFormIncidentMotivationTypeService;

        public MainFormAdminService(IDataAccess dataAccess,
                                         ConnectionStringData connectionStringData,
                                         IConfiguration configuration,
                                         IMainFormSampleCheckboxService mainFormSampleCheckboxService,
                                         IMainFormSampleCheckboxAdminService mainFormSampleCheckboxAdminService,

                                         IMainFormImpactedPersonTypeService mainFormImpactedPersonTypeService,
                                         IMainFormIncidentBehaviourTypeService mainFormIncidentBehaviourTypeService,
                                         IMainFormIncidentMotivationTypeService mainFormIncidentMotivationTypeService
                                         )
        {
            _dataAccess = dataAccess;
            _mainFormSampleCheckboxService = mainFormSampleCheckboxService;
            _connectionStringData = connectionStringData;
            _configuration = configuration;
            _mainFormSampleCheckboxAdminService = mainFormSampleCheckboxAdminService;
            _mainFormImpactedPersonTypeService = mainFormImpactedPersonTypeService;
            _mainFormIncidentBehaviourTypeService = mainFormIncidentBehaviourTypeService;
            _mainFormIncidentMotivationTypeService = mainFormIncidentMotivationTypeService;
        }




        public async Task<(List<MainFormAdminEntityModel> Items, int TotalCount)> GetAllOrByFilterAsync(
                                                                                        string? staffFullName,
                                                                                        int? statusId,
                                                                                        string? sortBy,
                                                                                        string? sortDirection,
                                                                                        int? pageNumber,
                                                                                        int? pageSize)
        {
            using var connection = new SqlConnection(
                _configuration.GetConnectionString(_connectionStringData.SqlConnectionName)
            );

            var parameters = new DynamicParameters();
            parameters.Add("@StaffFullName", staffFullName);
            parameters.Add("@StatusId", statusId);
            parameters.Add("@SortBy", sortBy ?? "DateSubmitted");
            parameters.Add("@SortDirection", sortDirection ?? "DESC");
            parameters.Add("@PageNumber", pageNumber ?? 1);
            parameters.Add("@PageSize", pageSize ?? 50);

            using var multi = await connection.QueryMultipleAsync(
                "dbo.spMainFormAdmin_GetAllOrByFilter",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var items = (await multi.ReadAsync<MainFormAdminEntityModel>()).ToList();
            var totalCount = await multi.ReadFirstAsync<int>();

            return (items, totalCount);
        }


        /// <summary>
        /// Retrieves a MainFormAdminEntityModel by its submission ID.
        /// </summary>
        /// <param name="submissionId">The ID of the submission to retrieve.</param>
        /// <returns>
        /// The matching MainFormAdminEntityModel if found; otherwise, null.
        /// </returns>
        public async Task<MainFormAdminEntityModel?> GetById(int submissionId)
        {
            var rows = await _dataAccess.LoadDataAsync<MainFormAdminEntityModel, dynamic>(
                                        "dbo.spMainFormAdmin_GetById",
                                        new
                                        {
                                            Id = submissionId
                                        },
                                        _connectionStringData.SqlConnectionName);

            // rows is a Task of List<> of type BiisStaffModel 
            // return a single BiisStaffModel
            return rows.FirstOrDefault();
        }



        public async Task<int> UpdateAsync(MainFormAdminEntityModel updated,
                                           List<MainFormAuditLogEntityModel> auditEntries)
        {
            // Update the submission record
            await _dataAccess.SaveDataAsync<dynamic>(
                    "dbo.spMainFormAdmin_Update",
                    new
                    {
                        Id = updated.Id,
                        StudentReferenceNumber = updated.StudentReferenceNumber,
                        StudentDateOfBirth = updated.StudentDateOfBirth,
                        SubmittedByWindowsUserName = updated.SubmittedByWindowsUserName,

                        // Textboxes
                        StudentFullName = updated.StudentFullName,
                        StaffFullName = updated.StaffFullName,
                        StaffTelephoneNumber = updated.StaffTelephoneNumber,
                        StaffEmail = updated.StaffEmail,
                        IncidentPersonName = updated.IncidentPersonName,
                        IncidentDate = updated.IncidentDate,
                        StaffMemberAssignedAdmin = updated.StaffMemberAssignedAdmin,

                        SampleTextbox = updated.SampleTextbox,
                        SampleDate = updated.SampleDate,
                        SampleTime = updated.SampleTime,
                        SampleCost = updated.SampleCost,
                        SampleTextboxAdmin = updated.SampleTextboxAdmin,
                        SampleDateAdmin = updated.SampleDateAdmin,
                        SampleCostAdmin = updated.SampleCostAdmin,


                        // Textareas
                        IncidentDetails = updated.IncidentDetails,
                        ActionTakenByCollegeAdmin = updated.ActionTakenByCollegeAdmin,
                        AdminNote = updated.AdminNote,
                        SampleTextarea = updated.SampleTextarea,
                        SampleTextareaAdmin = updated.SampleTextareaAdmin,


                        // DropDownList
                        StatusId = updated.StatusId,
                        SampleDropdownId = updated.SampleDropdownId,
                        SampleDropdownAdminId = updated.SampleDropdownAdminId,


                        // Radios
                        IncidentHappenedToId = updated.IncidentHappenedToId,
                        NumberOfPeopleImpactedId = updated.NumberOfPeopleImpactedId,
                        NumberOfPeopleCausedIncidentId = updated.NumberOfPeopleCausedIncidentId,
                        IncidentLocationId = updated.IncidentLocationId,
                        HasSimilarIncidentHappenedBeforeId = updated.HasSimilarIncidentHappenedBeforeId,
                        SampleRadioId = updated.SampleRadioId,
                        SampleRadioAdminId = updated.SampleRadioAdminId,


                        // Checkboxes
                        // Requires update to bridging tables - see below


                    },
                    _connectionStringData.SqlConnectionName);


            // Save audit logs if any
            if (auditEntries != null && auditEntries.Count > 0)
            {
                foreach (var entry in auditEntries)
                {
                    entry.MainFormId = updated.Id;
                    await _dataAccess.SaveDataAsync("dbo.spAuditLog_Insert", entry, _connectionStringData.SqlConnectionName);
                }
            }

            // Update selections in bridging tables
            await _mainFormImpactedPersonTypeService.CreateAndDeleteAsync(updated.Id, updated.SelectedImpactedPersonTypeIds);
            await _mainFormIncidentBehaviourTypeService.CreateAndDeleteAsync(updated.Id, updated.SelectedIncidentBehaviourTypeIds);
            await _mainFormIncidentMotivationTypeService.CreateAndDeleteAsync(updated.Id, updated.SelectedIncidentMotivationTypeIds);
            await _mainFormSampleCheckboxService.CreateAndDeleteAsync(updated.Id, updated.SelectedSampleCheckboxIds);
            await _mainFormSampleCheckboxAdminService.CreateAndDeleteAsync(updated.Id, updated.SelectedSampleCheckboxAdminIds);

            return updated.Id;
        }



        // Method updated to return nullable int (int?) to safely handle NULLs from the database
        public async Task<int?> GetSampleRadioAdminIdByMainFormId(int submissionId)
        {
            var parameters = new { Id = submissionId };

            // LoadDataAsync updated to use int? so Dapper can handle NULL values without throwing an exception
            var rows = await _dataAccess.LoadDataAsync<int?, dynamic>(
                                        "dbo.spMainForm_GetSampleRadioAdminIdByMainFormId",
                                        parameters,
                                        _connectionStringData.SqlConnectionName);

            // Return the first result, which may be null if the database value is NULL or no rows are returned
            return rows.FirstOrDefault();
        }





    }
}
