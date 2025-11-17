using AyrshireCollege.Biis.InputProcessingLibrary.InputProcessing.FileUploadServices;
using AyrshireCollege.Biis.InputProcessingLibrary.InputProcessing.InputCleaner;
using AyrshireCollege.Biis.UserIdentityLibrary.IdentityServices;
using DataAccessLibrary.DataServices;
using DataAccessLibrary.Models;
using EmailLibrary.Models;
using EmailLibrary.Services;
using FluentEmail.Core;
using Humanizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MvcUi.AuthorizationAttributes;
using MvcUi.Models;
using MvcUi.Services;
using Newtonsoft.Json.Linq;
using SharedServicesLibrary.AppSettings;
using SharedServicesLibrary.EmailServices;
using SharedServicesLibrary.FormHandlingServices;
using SharedServicesLibrary.FormPreparationServices;
using SharedViewModelLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using static Azure.Core.HttpHeader;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MvcUi.Controllers
{
    [AllowRoles("BiisAdmin", "Admin", "Staff")]
    public class MainFormController : Controller
    {
        private readonly IMainFormService _mainFormService;
        private readonly IInputSanitizer _inputSanitizer;
        private readonly IFileService _fileService;
        private readonly IStatusService _statusService;
        private readonly IMainFormViewModelPreparationService _mainFormViewModelPreparationService;
        private readonly IIdentityService _identityService;
        private readonly IFormHandlingService _formHandlingService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IAdminStaffService _adminStaffService;
        private readonly IPrepareFormSubmissionEmailNotificationService _prepareFormSubmissionEmailNotificationService;
        private readonly IAuditService _auditService;
        private readonly GlobalSettings _globalSettings;

        public MainFormController(IMainFormService mainFormService,
                                    IInputSanitizer inputSanitizer,
                                    IFileService fileService,
                                    IStatusService statusService,
                                    IMainFormViewModelPreparationService mainFormViewModelPreparationService,
                                    IIdentityService identityService,
                                    IFormHandlingService formHandlingService,
                                    IEmailService emailService,
                                    IConfiguration configuration,
                                    IAdminStaffService adminStaffService,
                                    IPrepareFormSubmissionEmailNotificationService prepareFormSubmissionEmailNotificationService,
                                    IOptions<GlobalSettings> globalSettings,
                                    IAuditService auditService
                                    )
        {
            _mainFormService = mainFormService;
            _inputSanitizer = inputSanitizer;
            _fileService = fileService;
            _statusService = statusService;
            _mainFormViewModelPreparationService = mainFormViewModelPreparationService;
            _identityService = identityService;
            _formHandlingService = formHandlingService;
            _emailService = emailService;
            _configuration = configuration;
            _adminStaffService = adminStaffService;
            _prepareFormSubmissionEmailNotificationService = prepareFormSubmissionEmailNotificationService;
            _auditService = auditService;
            _globalSettings = globalSettings.Value;
        }


        public IActionResult Index()
        {
            return View("Index");
        }


        public async Task<IActionResult> List()
        {
            var viewModelList = await _mainFormViewModelPreparationService.PrepareListViewModelAsync();

            ViewBag.LoggedInUser = await _identityService.GetFormattedUserNameAsync();
            ViewData["AppName"] = _globalSettings.MyGlobalName;

            return View("List", viewModelList);
        }




        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var mainFormViewModel = await _mainFormViewModelPreparationService.PrepareCreateViewModelAsync();
            mainFormViewModel.Mode = "Create";

            // Set StatusId to 1 by default so it can be submitted via a hidden field,
            // avoiding the need for user input while still satisfying the [Required] attribute.
            mainFormViewModel.StatusId = 1;

            ViewData["AppName"] = _globalSettings.MyGlobalName;

            var apiBaseUrl = _configuration["StudentLookupApiBaseUrl"];
            ViewData["StudentLookupApiBaseUrl"] = apiBaseUrl;

            ViewData["StudentLookupApiKey"] = _configuration["ApiKeys:StudentLookupApi"];

            // Pre-populate Fields
            mainFormViewModel.StaffFullName = await _identityService.GetFormattedUserNameAsync();
            mainFormViewModel.StaffEmail = await _identityService.GetUnformattedUserNameAsync() + "@ayrshire.ac.uk";

            return View("Create", mainFormViewModel);
        }




        [HttpPost]
        public async Task<IActionResult> Create(MainFormViewModel mainFormViewModel)
        {
            mainFormViewModel.Mode = "Create";

            var windowsName = await _identityService.GetUnformattedUserNameAsync(); // firstname.surname
            mainFormViewModel.SubmittedByWindowsUserName = windowsName;


            // Manually trigger validation
            _formHandlingService.ValidateModel(mainFormViewModel, ModelState);

            if (ModelState.IsValid == false)
            {
                // Populate the dropdown lists and checkbox groups (e.g., GenderOptions, CampusOptions, FeatureOptions, etc.)
                // Required when returning the view (e.g., after a validation error) so the form can re-render correctly
                mainFormViewModel = await _formHandlingService.PopulateDropdownsAndListsStaff(mainFormViewModel);

                return View("Create", mainFormViewModel);
            }

            // Populate the dropdown lists and checkbox groups (e.g., GenderOptions, CampusOptions, FeatureOptions, etc.)
            // Required within _formHandlingServices.PopulateDisplayProperties to populate convenience display properties
            mainFormViewModel = await _formHandlingService.PopulateDropdownsAndListsStaff(mainFormViewModel);

            // Populate convenience display properties (e.g., GenderDisplayName, CampusDisplayName, SelectedFeatureNames, etc.)
            // These are used for rendering readable values in views or email templates
            _formHandlingService.PopulateDisplayProperties(mainFormViewModel);

            // Map ViewModel to EntityModel
            MainFormEntityModel mainFormEntityModel = _formHandlingService.MapViewToEntity(mainFormViewModel);

            // Get resolved user display name (e.g., "Alice Smith" or "John Doe acting as Jane Roe")
            var changedBy = await _identityService.GetResolvedDisplayNameAsync();

            // Generate audit entries for the creation
            var creationAuditEntries = await _auditService.GenerateAuditLogEntriesForCreation(mainFormEntityModel, changedBy);


            // Save submission entity
            int newId = await _mainFormService.CreateAsync(mainFormEntityModel, creationAuditEntries);




            // Prepare then send the admin notification email content based on the view model and submission ID,
            await _prepareFormSubmissionEmailNotificationService.PrepareAdminCreateNotificationEmailAsync(mainFormViewModel, newId);

            // Prep then send confirmation email to user that has submitted new record.
            // Note mainFormViewModel.TargetAdminEmails contains active Student Experience Admins populated from call to PrepareAdminCreateNotificationEmailAsync()
            await _prepareFormSubmissionEmailNotificationService.PrepareUserCreateNotificationEmailAsync(mainFormViewModel, newId);

            return RedirectToAction("Index", "Home", new { Id = newId });
        }






 













    }
}


