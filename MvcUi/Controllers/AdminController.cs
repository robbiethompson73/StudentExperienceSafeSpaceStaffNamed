using AyrshireCollege.Biis.InputProcessingLibrary.InputProcessing.FileUploadServices;
using AyrshireCollege.Biis.InputProcessingLibrary.InputProcessing.InputCleaner;
using AyrshireCollege.Biis.UserIdentityLibrary.IdentityServices;
using ClosedXML.Excel;
using DataAccessLibrary.DataServices;
using DataAccessLibrary.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MvcUi.AuthorizationAttributes;
using MvcUi.Models;
using MvcUi.Services;
using SharedServicesLibrary.AdminStaffServices;
using SharedServicesLibrary.AppSettings;
using SharedServicesLibrary.EmailServices;
using SharedServicesLibrary.ExcelExportServices;
using SharedServicesLibrary.FormHandlingServices;
using SharedServicesLibrary.FormPreparationModels;
using SharedServicesLibrary.FormPreparationServices;
using SharedServicesLibrary.Identity;
using SharedViewModelLibrary.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;

namespace MvcUi.Controllers
{


    [AllowRoles("BiisAdmin","Admin")]
    public class AdminController : Controller
    {

        private readonly IMainFormService _mainFormService;
        private readonly IMainFormAdminService _mainFormAdminService;
        private readonly IInputSanitizer _inputSanitizer;
        private readonly IFileService _fileService;
        private readonly IMainFormSampleCheckboxService _mainFormSampleCheckboxService;
        private readonly ISampleCheckboxService _sampleCheckboxService;
        private readonly IStatusService _statusService;
        private readonly IMainFormAdminViewModelPreparationService _mainFormAdminViewModelPreparationService;
        private readonly IFormHandlingService _formHandlingService;
        private readonly IIdentityService _identityService;
        private readonly IAuditService _mainFormAdminAuditService;
        private readonly IExcelDataPreparationService _excelDataPreparationService;
        private readonly IExcelDownloadService _excelDownloadService;
        private readonly IAdminStaffManagementService _adminStaffManagementService;
        private readonly IMainFormSampleCheckboxAdminService _mainFormSampleCheckboxAdminService;
        private readonly GlobalSettings _globalSettings;


        public AdminController(IMainFormAdminService mainFormAdminService,
                                IInputSanitizer inputSanitizer,
                                IFileService fileService,
                                IMainFormSampleCheckboxService mainFormSampleCheckboxService,
                                ISampleCheckboxService sampleCheckboxService,
                                IStatusService statusService,
                                IMainFormAdminViewModelPreparationService mainFormAdminViewModelPreparationService,
                                IFormHandlingService formHandlingService,
                                IIdentityService identityService,
                                IOptions<GlobalSettings> globalSettings,
                                IAuditService mainFormAdminAuditService,
                                IExcelDataPreparationService excelDataPreparationService,
                                IExcelDownloadService excelDownloadService,
                                IAdminStaffManagementService adminStaffManagementService,
                                IMainFormSampleCheckboxAdminService mainFormSampleCheckboxAdminService
            )
        {
            _mainFormAdminService = mainFormAdminService;
            _inputSanitizer = inputSanitizer;
            _fileService = fileService;
            _mainFormSampleCheckboxService = mainFormSampleCheckboxService;
            _sampleCheckboxService = sampleCheckboxService;
            _statusService = statusService;
            _mainFormAdminViewModelPreparationService = mainFormAdminViewModelPreparationService;
            _formHandlingService = formHandlingService;
            _identityService = identityService;
            _mainFormAdminAuditService = mainFormAdminAuditService;
            _excelDataPreparationService = excelDataPreparationService;
            _excelDownloadService = excelDownloadService;
            _adminStaffManagementService = adminStaffManagementService;
            _mainFormSampleCheckboxAdminService = mainFormSampleCheckboxAdminService;
            _globalSettings = globalSettings.Value;
        }


        public async Task<IActionResult> Index(int? id)
        {
            var adminStaffViewModelList = await _adminStaffManagementService.GetAdminStaffAllAsync();

            if (id.HasValue)
            {
                // Pass a flag or message to the view indicating success
                ViewData["SuccessMessage"] = "Success";
            }

            ViewData["AppName"] = _globalSettings.MyGlobalName;

            return View("Index", adminStaffViewModelList);
        }

        /// <summary>
        /// Displays a paginated, filterable, and sortable list of form submissions for admins.
        /// Optionally shows a success message if redirected after an update or creation.
        /// </summary>
        /// <param name="id">Optional ID used to trigger success message display.</param>
        /// <param name="studentReferenceNumber">Optional filter to search by student reference number.</param>
        /// <param name="statusId">Optional filter to filter by status (e.g., Open, Closed).</param>
        /// <param name="sortBy">Column to sort by. Defaults to "DateSubmitted".</param>
        /// <param name="sortDirection">Sort direction: "ASC" or "DESC". Defaults to "DESC".</param>
        /// <param name="pageNumber">Current page number. Defaults to 1.</param>
        /// <param name="pageSize">Number of records per page. Defaults to 50.</param>
        /// <returns>List view with paged result set and filter/sort context.</returns>
        [HttpGet]
        public async Task<IActionResult> List(int? id,
                                              string? studentReferenceNumber,
                                              int? statusId,
                                              string? sortBy,
                                              string? sortDirection,
                                              int pageNumber = 1,
                                              int pageSize = 50)
        {
            // Set ViewData values to preserve user inputs and maintain context between requests (e.g., sorting, filtering, paging)
            ViewData["StudentReference"] = studentReferenceNumber ?? string.Empty;
            ViewData["StatusId"] = statusId;
            ViewData["AppName"] = _globalSettings.MyGlobalName;
            ViewData["SortBy"] = sortBy ?? "DateSubmitted";      // Default sort column
            ViewData["SortDirection"] = sortDirection ?? "DESC"; // Default sort direction
            ViewData["PageNumber"] = pageNumber;
            ViewData["PageSize"] = pageSize;

            // Prepare the paged, filtered, and sorted list of submissions via a service class
            var pagedResult = await _mainFormAdminViewModelPreparationService.PreparePagedListAdminViewModelAsync(
                studentReferenceNumber,
                statusId,
                sortBy,
                sortDirection,
                pageNumber,
                pageSize
            );

            // Provide total record count to the view for pagination display
            ViewData["RecordCount"] = pagedResult.TotalRecords;

            // If redirected from a create/update action (via an ID), show a success message
            if (id.HasValue)
            {
                ViewData["SuccessMessage"] = "Success";
            }

            // Return the List view with the prepared paged result
            return View("List", pagedResult);
        }




        public async Task<IActionResult> Details(int id)
        {
            var mainFormAdminViewModel = await _mainFormAdminViewModelPreparationService.PreparePopulatedAdminViewModelAsync(id);

            ViewData["AppName"] = _globalSettings.MyGlobalName;

            return View("Details", mainFormAdminViewModel);
        }




        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {

            // Step 1: Validate the ID early
            if (id <= 0)
            {
                return RedirectToAction("BadRequestPage", "Error"); // Optional if invalid ID
            }

            // Step 2: Check if the submission exists
            var mainFormAdminViewModel = await _mainFormAdminViewModelPreparationService.PreparePopulatedAdminViewModelAsync(id);

            if (mainFormAdminViewModel == null)
            {
                return RedirectToAction("NotFoundPage", "Error");
            }

            ViewData["AppName"] = _globalSettings.MyGlobalName;

            return View("Update", mainFormAdminViewModel);
        }



        [HttpPost]
        public async Task<IActionResult> Update(MainFormAdminViewModel mainFormAdminViewModel)
        {
            var existingSubmission = await _mainFormAdminService.GetById(mainFormAdminViewModel.Id);

            // Preserve Windows Name
            mainFormAdminViewModel.SubmittedByWindowsUserName = existingSubmission.SubmittedByWindowsUserName;

            // Copy admin-specific selection to the base class property
            mainFormAdminViewModel.StatusId = mainFormAdminViewModel.AdminStatusId ?? 0;

            // Clear the previous model state error (from earlier binding)
            ModelState.Remove(nameof(mainFormAdminViewModel.StatusId));


            // Manually trigger validation
            _formHandlingService.ValidateModel(mainFormAdminViewModel, ModelState);

            // Debug Validation
            //foreach (var kvp in ModelState)
            //{
            //    var key = kvp.Key;
            //    var errors = kvp.Value.Errors;
            //    if (errors.Count > 0)
            //    {
            //        foreach (var error in errors)
            //        {
            //            var errorMessage = error.ErrorMessage;
            //            // Log or inspect these messages to find out what is failing
            //            Debug.WriteLine($"{key}: {errorMessage}");
            //        }
            //    }
            //}



            if (ModelState.IsValid == false)
            {
                // Re-populate dropdowns and checkbox groups
                mainFormAdminViewModel = await _formHandlingService.PopulateDropdownsAndListsAdmin(mainFormAdminViewModel);

                return View("Update", mainFormAdminViewModel);
            }

            // Use MapViewModelToEntity to map the ViewModel to the Entity model
            MainFormAdminEntityModel updatedSubmission = _formHandlingService.MapViewToEntityAdmin(mainFormAdminViewModel);

            // Need to populate SelectedCheckboxIds as these are not populated from _mainFormAdminService.GetById
            existingSubmission.SelectedSampleCheckboxIds = await _mainFormSampleCheckboxService.GetSelectedSampleCheckboxIdsByMainFormIdAsync(mainFormAdminViewModel.Id);
            existingSubmission.SelectedSampleCheckboxAdminIds = await _mainFormSampleCheckboxAdminService.GetSelectedSampleCheckboxAdminIdsByMainFormIdAsync(mainFormAdminViewModel.Id);


            var auditEntries = await _mainFormAdminAuditService.GenerateAuditLogEntries(existingSubmission,
                                                                                        updatedSubmission,
                                                                                        await _identityService.GetResolvedDisplayNameAsync());

            // Persist the updated submission, including any changes and the appended audit log
            await _mainFormAdminService.UpdateAsync(updatedSubmission,
                                                    auditEntries
                                                    );



            // NO NOTIFICATION EMAILS SENT ON ADMIN UPDATE RECORD

            // Store a success message in TempData to persist it across the redirect
            TempData["SuccessMessage"] = "Success";

            return RedirectToAction("Update", new { id = existingSubmission.Id });
        }


        public async Task<IActionResult> ExportToExcel()
        {
            // Step 1: Retrieve data from the database via Dapper
            // This calls a stored procedure and maps the results to a list of ExcelEntityModel
            List<ExcelEntityModel> excelList = await _excelDownloadService.GetDataForExcelExport();

            // Step 2: Pass the data to the Excel generation service
            // This method takes the list of data and:
            // - Creates a new Excel workbook using ClosedXML
            // - Adds a worksheet and writes headers and rows
            // - Applies styling (e.g., bold headers, background color, borders)
            // - Saves the workbook to a memory stream and returns it as a byte array
            byte[] excelFile = _excelDataPreparationService.BuildExcelExportFile(excelList);

            // Step 3: Return the Excel file as a downloadable response
            // The File() helper creates a FileContentResult with the appropriate content type and filename
            return File(
                excelFile,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Export.xlsx");
        }








    }
}
