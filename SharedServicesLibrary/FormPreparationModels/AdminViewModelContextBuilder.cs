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
        private readonly IIncidentHappenedToService _incidentHappenedToService;
        private readonly INumberOfPeopleImpactedService _numberOfPeopleImpactedService;
        private readonly INumberOfPeopleCausedIncidentService _numberOfPeopleCausedIncidentService;
        private readonly IIncidentLocationService _incidentLocationService;
        private readonly IHasSimilarIncidentHappenedBeforeService _hasSimilarIncidentHappenedBeforeService;

        public AdminViewModelContextBuilder(IViewModelContextBuilder baseBuilder,
                                            IAuditLogService auditLogService,
                                            IMainFormService mainFormService,
                                            IMainFormAdminService mainFormAdminService,

                                            IIncidentHappenedToService incidentHappenedToService,
                                            INumberOfPeopleImpactedService numberOfPeopleImpactedService,
                                            INumberOfPeopleCausedIncidentService numberOfPeopleCausedIncidentService,
                                            IIncidentLocationService incidentLocationService,
                                            IHasSimilarIncidentHappenedBeforeService hasSimilarIncidentHappenedBeforeService
                                            )
        {
            _baseBuilder = baseBuilder;
            _auditLogService = auditLogService;
            _mainFormService = mainFormService;
            _mainFormAdminService = mainFormAdminService;
            _incidentHappenedToService = incidentHappenedToService;
            _numberOfPeopleImpactedService = numberOfPeopleImpactedService;
            _numberOfPeopleCausedIncidentService = numberOfPeopleCausedIncidentService;
            _incidentLocationService = incidentLocationService;
            _hasSimilarIncidentHappenedBeforeService = hasSimilarIncidentHappenedBeforeService;

            //_adminMetadataService = adminMetadataService;
        }

        public async Task<AdminViewModelMappingContext> BuildAsync(int submissionId)
        {

            // Create instance to access Selected ...Ids etc
            var existingSubmission = await _mainFormAdminService.GetById(submissionId);

            var baseContext = await _baseBuilder.BuildAsync(submissionId);


            // Radios
            // User
                var incidentHappenedToOptions = await _incidentHappenedToService.GetAllActiveSelectListAsync();
                var selectedIncidentHappenedToId = await _mainFormService.GetIncidentHappenedToIdByMainFormId(submissionId);

                // Find the matching option by comparing IDs
                var selectedIncidentHappenedTo = incidentHappenedToOptions
                    .FirstOrDefault(c => c.Value == selectedIncidentHappenedToId.ToString());

                // Get the option name (text), or null if not found
                var selectedIncidentHappenedToName = selectedIncidentHappenedTo?.Text;


                var numberOfPeopleImpactedOptions = await _numberOfPeopleImpactedService.GetAllActiveSelectListAsync();
                var selectedNumberOfPeopleImpactedId = await _mainFormService.GetNumberOfPeopleImpactedIdByMainFormId(submissionId);
                var selectedNumberOfPeopleImpacted = numberOfPeopleImpactedOptions.FirstOrDefault(c => c.Value == selectedNumberOfPeopleImpactedId.ToString());
                var selectedNumberOfPeopleImpactedName = selectedNumberOfPeopleImpacted?.Text;


                var numberOfPeopleCausedIncidentOptions = await _numberOfPeopleCausedIncidentService.GetAllActiveSelectListAsync();
                var selectedNumberOfPeopleCausedIncidentId = await _mainFormService.GetNumberOfPeopleCausedIncidentIdByMainFormId(submissionId);
                var selectedNumberOfPeopleCausedIncident = numberOfPeopleCausedIncidentOptions.FirstOrDefault(c => c.Value == selectedNumberOfPeopleCausedIncidentId.ToString());
                var selectedNumberOfPeopleCausedIncidentName = selectedNumberOfPeopleCausedIncident?.Text;

                var incidentLocationOptions = await _incidentLocationService.GetAllActiveSelectListAsync();
                var selectedIncidentLocationId = await _mainFormService.GetIncidentLocationIdByMainFormId(submissionId);
                var selectedIncidentLocation = incidentLocationOptions.FirstOrDefault(c => c.Value == selectedIncidentLocationId.ToString());
                var selectedIncidentLocationName = selectedIncidentLocation?.Text;

                var hasSimilarIncidentHappenedBeforeOptions = await _hasSimilarIncidentHappenedBeforeService.GetAllActiveSelectListAsync();
                var selectedHasSimilarIncidentHappenedBeforeId = await _mainFormService.GetHasSimilarIncidentHappenedBeforeIdByMainFormId(submissionId);
                var selectedHasSimilarIncidentHappenedBefore = hasSimilarIncidentHappenedBeforeOptions.FirstOrDefault(c => c.Value == selectedHasSimilarIncidentHappenedBeforeId.ToString());
                var selectedHasSimilarIncidentHappenedBeforeName = selectedHasSimilarIncidentHappenedBefore?.Text;


            // Radios
            // Admin


            // Dropdown Lists
            




            // Checkboxes


            // Get and format audit logs
            var auditLogs = await _auditLogService.GetAuditLogsByMainFormIdAsync(submissionId);
            var auditFormatted = string.Join("<br />", auditLogs
                .OrderByDescending(x => x.ChangeDate)
                .Select(log => $"{log.ChangeDate:yyyy-MM-dd HH:mm} by {log.ChangedBy} — {log.DisplayName}: '{log.OldValue}' → '{log.NewValue}'"));


            return new AdminViewModelMappingContext
            {

                // Radios
                IncidentHappenedToOptions = incidentHappenedToOptions,
                SelectedIncidentHappenedToId = selectedIncidentHappenedToId,
                SelectedIncidentHappenedToName = selectedIncidentHappenedToName,

                NumberOfPeopleImpactedOptions = numberOfPeopleImpactedOptions,
                SelectedNumberOfPeopleImpactedId = selectedNumberOfPeopleImpactedId,
                SelectedNumberOfPeopleImpactedName = selectedNumberOfPeopleImpactedName,

                NumberOfPeopleCausedIncidentOptions = numberOfPeopleCausedIncidentOptions,
                SelectedNumberOfPeopleCausedIncidentId = selectedNumberOfPeopleCausedIncidentId,
                SelectedNumberOfPeopleCausedIncidentName = selectedNumberOfPeopleCausedIncidentName,

                IncidentLocationOptions = incidentLocationOptions,
                SelectedIncidentLocationId = selectedIncidentLocationId,
                SelectedIncidentLocationName = selectedIncidentLocationName,

                HasSimilarIncidentHappenedBeforeOptions = hasSimilarIncidentHappenedBeforeOptions,
                SelectedHasSimilarIncidentHappenedBeforeId = selectedHasSimilarIncidentHappenedBeforeId,
                SelectedHasSimilarIncidentHappenedBeforeName = selectedHasSimilarIncidentHappenedBeforeName,


                // DropdownLists


                // Checkboxes
                ImpactedPersonTypeOptions = baseContext.ImpactedPersonTypeOptions,
                SelectedImpactedPersonTypeNames = baseContext.SelectedImpactedPersonTypeNames,

                IncidentBehaviourTypeOptions = baseContext.IncidentBehaviourTypeOptions,
                SelectedIncidentBehaviourTypeNames = baseContext.SelectedIncidentBehaviourTypeNames,

                IncidentMotivationTypeOptions = baseContext.IncidentMotivationTypeOptions,
                SelectedIncidentMotivationTypeNames = baseContext.SelectedIncidentMotivationTypeNames,



                // Predefined
                StatusOptions = baseContext.StatusOptions,

                // Populate the audit log HTML
                AuditFormattedHtml = auditFormatted

            };
        }




    }
}
