using DataAccessLibrary.DataServices;
using DataAccessLibrary.Models;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Microsoft.Extensions.Options;
using SharedViewModelLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;

namespace SharedServicesLibrary.FormPreparationModels
{
    public class ViewModelContextBuilder : IViewModelContextBuilder
    {
        private readonly ISampleCheckboxService _sampleCheckboxService;
        private readonly IStatusService _statusServices;
        private readonly IMainFormSampleCheckboxService _mainFormSampleCheckboxService;
        private readonly ISampleDropdownService _sampleDropdownService;
        private readonly IMainFormAdminService _mainFormAdminService;
        private readonly ISampleRadioService _sampleRadioService;
        private readonly IMainFormService _mainFormService;

        public ViewModelContextBuilder(ISampleCheckboxService sampleCheckboxService,
                                       IStatusService statusServices,
                                       IMainFormSampleCheckboxService mainFormSampleCheckboxService,
                                       ISampleDropdownService sampleDropdownService,
                                       IMainFormAdminService mainFormAdminService,
                                       ISampleRadioService sampleRadioService,
                                       IMainFormService mainFormService
                                       )
        {
            _sampleCheckboxService = sampleCheckboxService;
            _statusServices = statusServices;
            _mainFormSampleCheckboxService = mainFormSampleCheckboxService;
            _sampleDropdownService = sampleDropdownService;
            _mainFormAdminService = mainFormAdminService;
            _sampleRadioService = sampleRadioService;
            _mainFormService = mainFormService;
        }

        /// <summary>
        /// Builds a ViewModelMappingContext for a given submission, enriching it with selected and available features, films, and status.
        /// Use to populate a ViewModel with properties required for UI purposes that are not available in EntityModel
        /// </summary>
        /// <param name="submissionId">The ID of the submission to build the context for.</param>
        /// <returns>A ViewModelMappingContext containing selected IDs, option lists, and display names for use in UI rendering.</returns>
        public async Task<ViewModelMappingContext> BuildAsync(int submissionId)
        {
            // Create instance to access SelectedAwardTypeIds etc
            var existingSubmission = await _mainFormAdminService.GetById(submissionId);


            // Radios - User
            var sampleRadioOptions = await _sampleRadioService.GetAllActiveSelectListAsync();
            var selectedSampleRadioId = await _mainFormService.GetSampleRadioIdByMainFormId(submissionId);

            // Find the matching option by comparing IDs
            var selectedSampleRadio = sampleRadioOptions
                .FirstOrDefault(c => c.Value == selectedSampleRadioId.ToString());

            // Get the option name (text), or null if not found
            var selectedSampleRadioName = selectedSampleRadio?.Text;



            // DropdownLists
            var sampleDropdownOptions = await _sampleDropdownService.GetAllActiveSelectListAsync();



            // Checkboxes
            // The selectedAwardTypeIds are already stored in the entity model (existingSubmission.SelectedAwardTypeIds),
            // so this line has been removed to avoid redundant data fetching from the database.
            // Instead, the entity's SelectedAwardTypeIds should be mapped to the view model within the entity-to-view mapping method.
            // This preserves separation of concerns: the context builder enriches UI data (e.g., option lists and display names),
            // while the entity mapping method handles domain data like selected IDs.

            var sampleCheckboxOptions = await _sampleCheckboxService.GetAllActiveSelectListAsync();
            var selectedSampleCheckboxNames = sampleCheckboxOptions
                .Where(f => existingSubmission.SelectedSampleCheckboxIds.Contains(int.Parse(f.Value)))
                .Select(f => f.Text)
                .ToList();


            var statusOptions = await _statusServices.GetStatusAllActiveSelectListAsync();

            // Return a context object with all gathered data to be used for view model mapping or rendering
            return new ViewModelMappingContext
            {
                // Radios

                // Summary
                // Right now you’re mixing two styles (direct population vs context + mapper).
                // The optimal pattern is:
                // ContextBuilder always builds the lookup options + selected names.
                // Mapper maps entity + context → ViewModel.
                // Controller passes the ViewModel to the view.
                // This eliminates duplication (PopulateDropdownsAndListsStaff / PopulateDropdownsAndListsAdmin) and makes User and Admin consistent.

                // DropdownLists
                SampleDropdownOptions = sampleDropdownOptions,

                // Checkboxes
                SampleCheckboxOptions = sampleCheckboxOptions,
                SelectedSampleCheckboxNames = selectedSampleCheckboxNames,

                // Predefined
                StatusOptions = statusOptions
            };

        }


    }
}
