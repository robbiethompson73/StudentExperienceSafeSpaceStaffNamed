using DataAccessLibrary.DataServices;
using EmailLibrary.Configuration;
using EmailLibrary.Models;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EmailLibrary.Services
{
    public class EmailService : IEmailService
    {
        private readonly IFluentEmailFactory _emailFactory;
        private readonly IBiisAdminStaffService _biisAdminStaffServices;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<EmailService> _logger;
        private readonly EmailSettings _emailSettings;

        public EmailService(IFluentEmailFactory emailFactory,
                            IOptions<EmailSettings> emailSettings,
                            IBiisAdminStaffService biisAdminStaffServices,
                            IWebHostEnvironment environment,
                            ILogger<EmailService> logger)
        {
            _emailFactory = emailFactory;
            _biisAdminStaffServices = biisAdminStaffServices;
            _environment = environment;
            _logger = logger;
            _emailSettings = emailSettings.Value;
        }

        public async Task SendAdminCreateNotificationEmailAsync(FormSubmissionEmailNotificationAdminModel submissionModel)
        {
            // Determine the final list of admin recipients.
            // If recipient override is enabled (e.g., in development), return the override address from appsettings.Development;
            // otherwise, return the actual admin emails from the submission model.
            var finalAdminRecipients = ApplyAdminRecipientOverrideIfRequired(submissionModel);

            // Convert the list of admin email addresses into FluentEmail Address objects,
            // assigning the display name "Site Admin" to each recipient.
            // This allows a single email to be sent to all admins in the To field.
            var recipients = finalAdminRecipients
                                .Select(email => new Address(email, "Site Admin"))
                                .ToList();

            // Determine the CC recipients if enabled in the current environment
            List<string> ccRecipients = new();

            if (IsBiisAdminCcEnabledForCurrentEnvironment())
            {
                ccRecipients = await _biisAdminStaffServices.GetBiisAdminStaffActiveEmails();
            }

            var email = _emailFactory.Create()
                .To(recipients)
                .Subject(submissionModel.Subject)
                .UsingTemplateFromFile(submissionModel.TemplatePath, submissionModel);

            // Prevent duplicates
            foreach (var ccEmail in ccRecipients.Distinct())
            {
                if (!email.Data.CcAddresses.Any(e => e.EmailAddress.Equals(ccEmail, StringComparison.OrdinalIgnoreCase)))
                {
                    email.CC(ccEmail);
                }
            }

            await email.SendAsync();
        }



        private List<string> ApplyAdminRecipientOverrideIfRequired(FormSubmissionEmailNotificationAdminModel submissionModel)
        {
            var originalRecipients = submissionModel.TargetAdminEmails;

            // Check if the override flag is set in class EmailSettings and if an override email is provided
            if (_emailSettings.OverrideAdminRecipients && !string.IsNullOrWhiteSpace(_emailSettings.AdminEmailOverride))
            {
                // Save the real recipient list so it can be shown in test emails or logs for transparency
                submissionModel.OriginalRecipientAdminEmail = string.Join(", ", originalRecipients);

                // Return only the test override recipient set in appsettings
                return new List<string> { _emailSettings.AdminEmailOverride };
            }

            // Return the actual recipients if override is not enabled
            return originalRecipients;
        }


        /// <summary>
        /// Determines whether BIIS admin should be CC'd based on the current hosting environment.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the current environment is listed in the <c>SendBiisAdminCcEnvironments</c> configuration;
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This uses the environment name provided by <see cref="IWebHostEnvironment.EnvironmentName"/> and checks if it
        /// exists in the list configured in appsettings under <c>EmailSettings:SendBiisAdminCcEnvironments</c>.
        /// </remarks>
        private bool IsBiisAdminCcEnabledForCurrentEnvironment()
        {
            // Get the current environment name (e.g., "Development", "Production", "UAT")
            var currentEnvironment = _environment.EnvironmentName;

            // Check if the current environment is listed in the allowed CC environments
            return _emailSettings.SendBiisAdminCcEnvironments
                                 .Any(env => string.Equals(env, currentEnvironment, StringComparison.OrdinalIgnoreCase));
        }




        public async Task SendUserCreateNotificationEmailAsync(FormSubmissionEmailNotificationUserModel submissionModel)
        {
            // Determine the recipient(s) if enabled in the current environment
            var recipient = ApplyUserRecipientOverrideIfRequired(submissionModel);

            // Determine the CC recipients if enabled in the current environment
            List<string> ccRecipients = new();
            if (IsBiisAdminCcEnabledForCurrentEnvironment())
            {
                ccRecipients = await _biisAdminStaffServices.GetBiisAdminStaffActiveEmails();
            }

            // Log recipients before sending
            _logger.LogInformation("Sending user confirmation email to: {Recipient}", string.Join(", ", recipient));
            _logger.LogInformation("CC'ing the following BIIS Admins: {CC}", string.Join(", ", ccRecipients));


            var email = _emailFactory.Create()
                .To(recipient)
                .Subject(submissionModel.Subject)
                .UsingTemplateFromFile(submissionModel.TemplatePath, submissionModel);

            // Prevent duplicates
            foreach (var ccEmail in ccRecipients.Distinct())
            {
                if (!email.Data.CcAddresses.Any(e => e.EmailAddress.Equals(ccEmail, StringComparison.OrdinalIgnoreCase)))
                {
                    email.CC(ccEmail);
                }
            }

            // Log final To and CC addresses before sending
            _logger.LogInformation("Final To: {To}", string.Join(", ", email.Data.ToAddresses.Select(a => a.EmailAddress)));
            _logger.LogInformation("Final CC: {CC}", string.Join(", ", email.Data.CcAddresses.Select(a => a.EmailAddress)));

            await email.SendAsync();
        }


        private string ApplyUserRecipientOverrideIfRequired(FormSubmissionEmailNotificationUserModel submissionModel)
        {
            var originalRecipient = submissionModel.TargetUserEmail;

            // Check if the override flag is set in the email settings and if an override email is provided
            if (_emailSettings.OverrideUserRecipients && !string.IsNullOrWhiteSpace(_emailSettings.UserEmailOverride))
            {
                // Preserve original recipients for visibility in test email
                submissionModel.OriginalRecipientUserEmail = submissionModel.TargetUserEmail;

                // Return only the test override recipient (i.e., an email address set in the settings)
                return _emailSettings.UserEmailOverride;
            }

            // Return the actual recipients if override is not enabled
            return originalRecipient;
        }



    }
}
