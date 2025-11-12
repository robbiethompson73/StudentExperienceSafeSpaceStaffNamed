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
        private readonly IMainFormSampleCheckboxService _mainFormSampleCheckboxServices;
        private readonly ConnectionStringData _connectionStringData;
        private readonly IConfiguration _configuration;
        private readonly IMainFormSampleCheckboxAdminService _mainFormSampleCheckboxAdminService;

        public MainFormAdminService(IDataAccess dataAccess,
                                         IMainFormSampleCheckboxService mainFormSampleCheckboxServices,
                                         ConnectionStringData connectionStringData,
                                         IConfiguration configuration,
                                         IMainFormSampleCheckboxService mainFormSampleCheckboxService,
                                         IMainFormSampleCheckboxAdminService mainFormSampleCheckboxAdminService)
        {
            _dataAccess = dataAccess;
            _mainFormSampleCheckboxServices = mainFormSampleCheckboxServices;
            _connectionStringData = connectionStringData;
            _configuration = configuration;
            _mainFormSampleCheckboxAdminService = mainFormSampleCheckboxAdminService;
        }




        public async Task<(List<MainFormAdminEntityModel> Items, int TotalCount)> GetAllOrByFilterAsync(
                                                                                        string? studentReferenceNumber,
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
            parameters.Add("@StudentReferenceNumber", studentReferenceNumber);
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
                        SampleTextbox = updated.SampleTextbox,
                        SampleDate = updated.SampleDate,
                        SampleTime = updated.SampleTime,
                        SampleCost = updated.SampleCost,
                        SampleTextboxAdmin = updated.SampleTextboxAdmin,
                        SampleDateAdmin = updated.SampleDateAdmin,
                        SampleCostAdmin = updated.SampleCostAdmin,


                        // Textareas
                        SampleTextarea = updated.SampleTextarea,
                        SampleTextareaAdmin = updated.SampleTextareaAdmin,
                        AdminNote = updated.AdminNote,


                        // DropDownList
                        StatusId = updated.StatusId,
                        SampleDropdownId = updated.SampleDropdownId,
                        SampleDropdownAdminId = updated.SampleDropdownAdminId,


                        // Radios
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
            await _mainFormSampleCheckboxServices.CreateAndDeleteAsync(updated.Id, updated.SelectedSampleCheckboxIds);
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
