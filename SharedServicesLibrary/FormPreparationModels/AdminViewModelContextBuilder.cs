using DataAccessLibrary.DataServices;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServicesLibrary.FormPreparationModels
{
    public class AdminViewModelContextBuilder : IAdminViewModelContextBuilder
    {

        private readonly IViewModelContextBuilder _baseBuilder;
        private readonly IAuditLogService _auditLogService;
        private readonly IMainFormService _mainFormService;
        private readonly IMainFormAdminService _mainFormAdminService;
        private readonly ISampleRadioService _sampleRadioService;
        private readonly ISampleRadioAdminService _sampleRadioAdminService;
        private readonly ISampleCheckboxAdminService _sampleCheckboxAdminService;
        private readonly ISampleDropdownAdminService _sampleDropdownAdminService;

        public AdminViewModelContextBuilder(IViewModelContextBuilder baseBuilder,
                                            IAuditLogService auditLogService,
                                            IMainFormService mainFormService,
                                            IMainFormAdminService mainFormAdminService,
                                            ISampleRadioService sampleRadioService,
                                            ISampleRadioAdminService sampleRadioAdminService,
                                            ISampleCheckboxAdminService sampleCheckboxAdminService,
                                            ISampleDropdownAdminService sampleDropdownAdminService
                                            )
        {
            _baseBuilder = baseBuilder;
            _auditLogService = auditLogService;
            _mainFormService = mainFormService;
            _mainFormAdminService = mainFormAdminService;
            _sampleRadioService = sampleRadioService;
            _sampleRadioAdminService = sampleRadioAdminService;
            _sampleCheckboxAdminService = sampleCheckboxAdminService;
            _sampleDropdownAdminService = sampleDropdownAdminService;

            //_adminMetadataService = adminMetadataService;
        }

        public async Task<AdminViewModelMappingContext> BuildAsync(int submissionId)
        {

            // Create instance to access Selected ...Ids etc
            var existingSubmission = await _mainFormAdminService.GetById(submissionId);

            var baseContext = await _baseBuilder.BuildAsync(submissionId);


            // Radios
            // User
                var sampleRadioOptions = await _sampleRadioService.GetAllActiveSelectListAsync();
                var selectedSampleRadioId = await _mainFormService.GetSampleRadioIdByMainFormId(submissionId);

                // Find the matching option by comparing IDs
                var selectedSampleRadio = sampleRadioOptions
                    .FirstOrDefault(c => c.Value == selectedSampleRadioId.ToString());

                // Get the option name (text), or null if not found
                var selectedSampleRadioName = selectedSampleRadio?.Text;

            // Radios
            // Admin
                var sampleRadioAdminOptions = await _sampleRadioAdminService.GetAllActiveSelectListAsync();
                int? selectedSampleRadioAdminId = await _mainFormAdminService.GetSampleRadioAdminIdByMainFormId(submissionId);

                // Find the matching option by comparing IDs
                var selectedSampleRadioAdmin = sampleRadioAdminOptions
                    .FirstOrDefault(c => c.Value == selectedSampleRadioAdminId.ToString());

                // Get the option name (text), or null if not found
                var selectedSampleRadioAdminName = selectedSampleRadioAdmin?.Text;


            // Dropdown Lists
            var sampleDropdownAdminOptions = await _sampleDropdownAdminService.GetAllActiveSelectListAsync();





            // Checkboxes
            var sampleCheckboxAdminOptions = await _sampleCheckboxAdminService.GetAllActiveSelectListAsync();
            var selectedSampleCheckboxAdminNames = sampleCheckboxAdminOptions
                .Where(f => existingSubmission.SelectedSampleCheckboxAdminIds.Contains(int.Parse(f.Value)))
                .Select(f => f.Text)
                .ToList();



            // Get and format audit logs
            var auditLogs = await _auditLogService.GetAuditLogsByMainFormIdAsync(submissionId);
            var auditFormatted = string.Join("<br />", auditLogs
                .OrderByDescending(x => x.ChangeDate)
                .Select(log => $"{log.ChangeDate:yyyy-MM-dd HH:mm} by {log.ChangedBy} — {log.DisplayName}: '{log.OldValue}' → '{log.NewValue}'"));


            return new AdminViewModelMappingContext
            {

                // Radios
                SampleRadioOptions = sampleRadioOptions,
                SelectedSampleRadioId = selectedSampleRadioId,
                SelectedSampleRadioName = selectedSampleRadioName,

                SampleRadioAdminOptions = sampleRadioAdminOptions,
                SelectedSampleRadioAdminId = selectedSampleRadioAdminId,
                SelectedSampleRadioAdminName = selectedSampleRadioAdminName,



                // DropdownLists
                SampleDropdownOptions = baseContext.SampleDropdownOptions,
                SampleDropdownAdminOptions = sampleDropdownAdminOptions,


                // Checkboxes
                SampleCheckboxOptions = baseContext.SampleCheckboxOptions,
                SelectedSampleCheckboxNames = baseContext.SelectedSampleCheckboxNames,

                SampleCheckboxAdminOptions = sampleCheckboxAdminOptions,
                SelectedSampleCheckboxAdminNames = selectedSampleCheckboxAdminNames,




                // Predefined
                StatusOptions = baseContext.StatusOptions,

                // Populate the audit log HTML
                AuditFormattedHtml = auditFormatted

            };
        }




    }
}
