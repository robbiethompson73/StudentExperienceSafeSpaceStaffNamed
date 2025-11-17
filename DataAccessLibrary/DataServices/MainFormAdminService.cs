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
        private readonly ConnectionStringData _connectionStringData;
        private readonly IConfiguration _configuration;
        private readonly IMainFormImpactedPersonTypeService _mainFormImpactedPersonTypeService;
        private readonly IMainFormIncidentBehaviourTypeService _mainFormIncidentBehaviourTypeService;
        private readonly IMainFormIncidentMotivationTypeService _mainFormIncidentMotivationTypeService;

        public MainFormAdminService(IDataAccess dataAccess,
                                         ConnectionStringData connectionStringData,
                                         IConfiguration configuration,

                                         IMainFormImpactedPersonTypeService mainFormImpactedPersonTypeService,
                                         IMainFormIncidentBehaviourTypeService mainFormIncidentBehaviourTypeService,
                                         IMainFormIncidentMotivationTypeService mainFormIncidentMotivationTypeService
                                         )
        {
            _dataAccess = dataAccess;
            _connectionStringData = connectionStringData;
            _configuration = configuration;
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
                        SubmittedByWindowsUserName = updated.SubmittedByWindowsUserName,

                        // Textboxes
                        StaffFullName = updated.StaffFullName,
                        StaffTelephoneNumber = updated.StaffTelephoneNumber,
                        StaffEmail = updated.StaffEmail,
                        IncidentPersonName = updated.IncidentPersonName,
                        IncidentDate = updated.IncidentDate,
                        StaffMemberAssignedAdmin = updated.StaffMemberAssignedAdmin,

                        // Textareas
                        IncidentDetails = updated.IncidentDetails,
                        ActionTakenByCollegeAdmin = updated.ActionTakenByCollegeAdmin,
                        AdminNote = updated.AdminNote,


                        // DropDownList
                        StatusId = updated.StatusId,


                        // Radios
                        IncidentHappenedToId = updated.IncidentHappenedToId,
                        NumberOfPeopleImpactedId = updated.NumberOfPeopleImpactedId,
                        NumberOfPeopleCausedIncidentId = updated.NumberOfPeopleCausedIncidentId,
                        IncidentLocationId = updated.IncidentLocationId,
                        HasSimilarIncidentHappenedBeforeId = updated.HasSimilarIncidentHappenedBeforeId,


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

            return updated.Id;
        }








    }
}
