using AyrshireCollege.Biis.UserIdentityLibrary.IdentityServices;
using AyrshireCollege.Biis.PresentationFormattingLibrary.Date;
using DataAccessLibrary.DataServices;
using DataAccessLibrary.Models;
using EmailLibrary.Models;
using EmailLibrary.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SharedServicesLibrary.AppSettings;
using SharedViewModelLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SharedServicesLibrary.EmailServices
{
    public class PrepareFormSubmissionEmailNotificationService : IPrepareFormSubmissionEmailNotificationService
    {
        private readonly IEmailService _emailService;
        private readonly IAdminStaffService _adminStaffService;
        private readonly IIdentityService _identityService;
        private readonly IConfiguration _configuration;
        private readonly IMainFormService _submissionFlatService;
        private readonly ISampleDropdownService _sampleDropdownService;
        private readonly GlobalSettings _globalSettings;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public PrepareFormSubmissionEmailNotificationService(IEmailService emailService,
                                                IAdminStaffService adminStaffService,
                                                IIdentityService identityService,
                                                IConfiguration configuration,
                                                IMainFormService submissionFlatService,
                                                ISampleDropdownService sampleDropdownService,
                                                IOptions<GlobalSettings> globalSettings,
                                                IWebHostEnvironment webHostEnvironment
                                                )
        {
            _emailService = emailService;
            _adminStaffService = adminStaffService;
            _identityService = identityService;
            _configuration = configuration;
            _submissionFlatService = submissionFlatService;
            _sampleDropdownService = sampleDropdownService;
            _globalSettings = globalSettings.Value;
            _webHostEnvironment = webHostEnvironment;

        }

        /// <summary>
        /// Prepares the email notification model for an admin when a new main form submission is created,
        /// and delegates the sending of the email to the email service.
        /// </summary>
        /// <param name="viewModel">The MainFormViewModel view model containing the submitted form data.</param>
        /// <param name="submissionId">The unique identifier of the submitted form.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task PrepareAdminCreateNotificationEmailAsync(MainFormViewModel viewModel, int submissionId)
        {
            var baseUrl = _configuration["BaseUrl"] ?? throw new InvalidOperationException("BaseUrl is not configured.");
            var targetAdminEmails = await _adminStaffService.GetAdminStaffActiveEmails();
            var globalFormName = _globalSettings.MyGlobalName;

            var templatePath = _configuration["EmailTemplates:NotifyAdminFormCreate"]
                                    ?? throw new InvalidOperationException("Email template path is not configured.");


            var formattedUserName = await _identityService.GetResolvedDisplayNameAsync();

            // EmailLibrary.Models.FormSubmissionEmailNotificationAdminModel.cs
            var emailNotificationAdminModel = new FormSubmissionEmailNotificationAdminModel(baseUrl)
            {
                FormName = globalFormName,
                Subject = globalFormName + " Create Record",
                TemplatePath = templatePath,

                TargetAdminEmails = targetAdminEmails,
                // OriginalRecipientAdminEmail populated in EmailLibrary.EmailService.SendAdminCreateNotificationEmailAsync()

                Id = submissionId,
                SubmittedBy = formattedUserName,
                StudentFullName = viewModel.StudentFullName,
                StudentReferenceNumber = viewModel.StudentReferenceNumber,
                StudentDateOfBirth = viewModel.StudentDateOfBirth.HasValue
                                                      ? DateFormatting.ToLongOrdinalDate(viewModel.StudentDateOfBirth.Value)
                                                      : null

                //,SampleTextarea = viewModel.SampleTextarea,
                //SampleDropdownDisplayName = viewModel.SampleDropdownDisplayName,
                //SelectedSampleCheckboxNames = viewModel.SelectedSampleCheckboxNames,
                //SampleRadio = viewModel.SampleRadio
            };

            await _emailService.SendAdminCreateNotificationEmailAsync(emailNotificationAdminModel);
        }




        public async Task PrepareUserCreateNotificationEmailAsync(MainFormViewModel viewModel, int submissionId)
        {
            var collegeEmailDomain = _configuration["CollegeEmailDomain"]
                                ?? throw new InvalidOperationException("College Email Domain is not configured.");
            var targetUserEmail = viewModel.SubmittedByWindowsUserName + collegeEmailDomain;
            var globalFormName = _globalSettings.MyGlobalName;
            var templatePath = _configuration["EmailTemplates:NotifyUserFormCreate"]
                                ?? throw new InvalidOperationException("Email template path is not configured.");

            var formattedUserName = await _identityService.GetFormattedUserNameAsync();

            // EmailLibrary.Models.FormSubmissionEmailNotificationUserModel.cs
            var emailNotificationUserModel = new FormSubmissionEmailNotificationUserModel()
            {
                FormName = globalFormName,
                Subject = globalFormName + " Create Record",
                TemplatePath = templatePath,

                SubmittedBy = formattedUserName,
                TargetUserEmail = targetUserEmail,

                Id = submissionId,
                StudentFullName = viewModel.StudentFullName,
                StudentReferenceNumber = viewModel.StudentReferenceNumber

                //,SampleTextarea = viewModel.SampleTextarea
            };

            await _emailService.SendUserCreateNotificationEmailAsync(emailNotificationUserModel);
        }



    }
}
