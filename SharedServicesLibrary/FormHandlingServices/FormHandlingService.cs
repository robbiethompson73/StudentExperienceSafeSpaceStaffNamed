using AyrshireCollege.Biis.InputProcessingLibrary.InputProcessing.FileUploadServices;
using AyrshireCollege.Biis.InputProcessingLibrary.InputProcessing.InputCleaner;
using AyrshireCollege.Biis.PresentationFormattingLibrary.Strings;
using DataAccessLibrary.DataServices;
using DataAccessLibrary.Models;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using SharedServicesLibrary.FormPreparationModels;
using SharedViewModelLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SharedServicesLibrary.FormHandlingServices
{
    public class FormHandlingService : IFormHandlingService
    {
        private readonly IStatusService _statusService;
        private readonly IFileUploadHelper _fileUploadHelper;
        private readonly IInputSanitizer _inputSanitizer;
        private readonly IIncidentHappenedToService _incidentHappenedToService;
        private readonly INumberOfPeopleImpactedService _numberOfPeopleImpactedService;
        private readonly INumberOfPeopleCausedIncidentService _numberOfPeopleCausedIncidentService;
        private readonly IIncidentLocationService _incidentLocationService;
        private readonly IHasSimilarIncidentHappenedBeforeService _hasSimilarIncidentHappenedBeforeService;
        private readonly IImpactedPersonTypeService _impactedPersonTypeService;
        private readonly IIncidentBehaviourTypeService _incidentBehaviourTypeService;
        private readonly IIncidentMotivationTypeService _incidentMotivationTypeService;
        private readonly IMainFormImpactedPersonTypeService _mainFormImpactedPersonTypeService;
        private readonly IMainFormIncidentBehaviourTypeService _mainFormIncidentBehaviourTypeService;
        private readonly IMainFormIncidentMotivationTypeService _mainFormIncidentMotivationTypeService;

        public FormHandlingService(IStatusService statusService,
                                   IFileUploadHelper fileUploadHelper,
                                   IInputSanitizer inputSanitizer,
                                   
                                    IIncidentHappenedToService incidentHappenedToService,
                                    INumberOfPeopleImpactedService numberOfPeopleImpactedService,
                                    INumberOfPeopleCausedIncidentService numberOfPeopleCausedIncidentService,
                                    IIncidentLocationService incidentLocationService,
                                    IHasSimilarIncidentHappenedBeforeService hasSimilarIncidentHappenedBeforeService,

                                    IImpactedPersonTypeService impactedPersonTypeService,
                                    IIncidentBehaviourTypeService incidentBehaviourTypeService,
                                    IIncidentMotivationTypeService incidentMotivationTypeService,

                                   IMainFormImpactedPersonTypeService mainFormImpactedPersonTypeService,
                                   IMainFormIncidentBehaviourTypeService mainFormIncidentBehaviourTypeService,
                                   IMainFormIncidentMotivationTypeService mainFormIncidentMotivationTypeService
                                   )
        {
            _statusService = statusService;
            _fileUploadHelper = fileUploadHelper;
            _inputSanitizer = inputSanitizer;
            _incidentHappenedToService = incidentHappenedToService;
            _numberOfPeopleImpactedService = numberOfPeopleImpactedService;
            _numberOfPeopleCausedIncidentService = numberOfPeopleCausedIncidentService;
            _incidentLocationService = incidentLocationService;
            _hasSimilarIncidentHappenedBeforeService = hasSimilarIncidentHappenedBeforeService;
            _impactedPersonTypeService = impactedPersonTypeService;
            _incidentBehaviourTypeService = incidentBehaviourTypeService;
            _incidentMotivationTypeService = incidentMotivationTypeService;
            _mainFormImpactedPersonTypeService = mainFormImpactedPersonTypeService;
            _mainFormIncidentBehaviourTypeService = mainFormIncidentBehaviourTypeService;
            _mainFormIncidentMotivationTypeService = mainFormIncidentMotivationTypeService;
        }



        // Method to validate model and add errors to ModelState
        public void ValidateModel(IValidatableObject model, ModelStateDictionary modelState)
        {
            var validationResults = model.Validate(new ValidationContext(model));

            foreach (var validationResult in validationResults)
            {
                foreach (var memberName in validationResult.MemberNames)
                {
                    modelState.AddModelError(memberName, validationResult.ErrorMessage);
                }
            }
        }

        /// <summary>
        /// Populates dropdowns, checkbox groups, and other selection lists required by the view model.
        /// This method retrieves active data entries from underlying services and formats them as
        /// <see cref="Microsoft.AspNetCore.Mvc.Rendering.SelectListItem"/> collections suitable for rendering UI controls (e.g., <select>, checkboxes, radio buttons).
        /// 
        /// Additionally, if the user has already selected values (e.g., via form submission),
        /// the method resolves and assigns the corresponding human-readable display names to support
        /// details views, summary screens, or email templates.
        /// 
        /// This ensures that:
        /// - All selection controls are pre-filled with current, valid options.
        /// - UI elements like dropdowns and checkbox groups are bound to strongly-typed lists.
        /// - Display names are available for selected values where appropriate.
        /// 
        /// Usage: Called before rendering Create, Edit, or Details views to ensure view model completeness.
        /// </summary>
        public async Task<MainFormViewModel> PopulateDropdownsAndListsStaff(MainFormViewModel viewModel)
        {

            // Populate the corresponding SelectList property on the view model with active options,
            // formatted as SelectListItems (with Value and Text). This list is used to render a dropdown in the UI,
            // where each item includes:
            // - Value: the unique identifier (as a string)
            // - Text: the human-readable display name
            //
            // The underlying service method (e.g., GetAllActiveSelectListAsync) retrieves only items
            // marked as active, ensuring the dropdown contains valid and current selections.
            //
            // Example usage in the view:
            // @Html.DropDownListFor(m => m.Selected[Entity]Id, Model.[Entity]Options, "Select an option")

            // Radios
            viewModel.IncidentHappenedToOptions = await _incidentHappenedToService.GetAllActiveSelectListAsync();
            viewModel.NumberOfPeopleImpactedOptions = await _numberOfPeopleImpactedService.GetAllActiveSelectListAsync();
            viewModel.NumberOfPeopleCausedIncidentOptions = await _numberOfPeopleCausedIncidentService.GetAllActiveSelectListAsync();
            viewModel.IncidentLocationOptions = await _incidentLocationService.GetAllActiveSelectListAsync();
            viewModel.HasSimilarIncidentHappenedBeforeOptions = await _hasSimilarIncidentHappenedBeforeService.GetAllActiveSelectListAsync();


            // DropDownlist


            // If an ID is selected, attempt to resolve its corresponding display name
            // by searching the associated SelectList.
            // This allows the application to display a human-readable label (e.g., on Details views or in emails)
            // instead of just showing the underlying ID value.
            if (viewModel.SelectedIncidentHappenedToId.HasValue)
            {
                // Find the matching SelectListItem using the selected ID (converted to string for comparison),
                // as SelectListItem.Value is always a string.
                var selectedIncidentHappenedTo = viewModel.IncidentHappenedToOptions
                    .FirstOrDefault(option => option.Value == viewModel.SelectedIncidentHappenedToId.Value.ToString());

                // If a matching item is found, assign its display text to ItemName.
                // This is used in Details views to show the human-readable item name.
                viewModel.IncidentHappenedToName = selectedIncidentHappenedTo?.Text;
            }



            if (viewModel.SelectedNumberOfPeopleImpactedId.HasValue)
            {
                var selectedNumberOfPeopleImpacted = viewModel.NumberOfPeopleImpactedOptions
                    .FirstOrDefault(option => option.Value == viewModel.SelectedNumberOfPeopleImpactedId.Value.ToString());
                viewModel.NumberOfPeopleImpactedName = selectedNumberOfPeopleImpacted?.Text;
            }


            if (viewModel.SelectedNumberOfPeopleCausedIncidentId.HasValue)
            {
                var selectedNumberOfPeopleCausedIncident = viewModel.NumberOfPeopleCausedIncidentOptions
                    .FirstOrDefault(option => option.Value == viewModel.SelectedNumberOfPeopleCausedIncidentId.Value.ToString());
                viewModel.NumberOfPeopleCausedIncidentName = selectedNumberOfPeopleCausedIncident?.Text;
            }

            if (viewModel.SelectedIncidentLocationId.HasValue)
            {
                var selectedIncidentLocation = viewModel.IncidentLocationOptions
                    .FirstOrDefault(option => option.Value == viewModel.SelectedIncidentLocationId.Value.ToString());
                viewModel.IncidentLocationName = selectedIncidentLocation?.Text;
            }

            if (viewModel.SelectedHasSimilarIncidentHappenedBeforeId.HasValue)
            {
                var selectedHasSimilarIncidentHappenedBefore = viewModel.HasSimilarIncidentHappenedBeforeOptions
                    .FirstOrDefault(option => option.Value == viewModel.SelectedHasSimilarIncidentHappenedBeforeId.Value.ToString());
                viewModel.HasSimilarIncidentHappenedBeforeName = selectedHasSimilarIncidentHappenedBefore?.Text;
            }




            // Checkbox group
            // Because the selected values are processed and displayed directly in the view
            // by comparing Selected[Entity]Ids to the SelectListItems, there is no need
            // to separately resolve and store the human-readable names on the server.
            viewModel.ImpactedPersonTypeOptions = await _impactedPersonTypeService.GetAllActiveSelectListAsync();
            viewModel.IncidentBehaviourTypeOptions = await _incidentBehaviourTypeService.GetAllActiveSelectListAsync();
            viewModel.IncidentMotivationTypeOptions = await _incidentMotivationTypeService.GetAllActiveSelectListAsync();

            // Predefined
            viewModel.StatusOptions = await _statusService.GetStatusAllActiveSelectListAsync();

            return viewModel;
        }

        public async Task<MainFormAdminViewModel> PopulateDropdownsAndListsAdmin(MainFormAdminViewModel viewModel)
        {
            // Radios
            viewModel.IncidentHappenedToOptions = await _incidentHappenedToService.GetAllActiveSelectListAsync();
            viewModel.NumberOfPeopleImpactedOptions = await _numberOfPeopleImpactedService.GetAllActiveSelectListAsync();
            viewModel.NumberOfPeopleCausedIncidentOptions = await _numberOfPeopleCausedIncidentService.GetAllActiveSelectListAsync();
            viewModel.IncidentLocationOptions = await _incidentLocationService.GetAllActiveSelectListAsync();
            viewModel.HasSimilarIncidentHappenedBeforeOptions = await _hasSimilarIncidentHappenedBeforeService.GetAllActiveSelectListAsync();


            // Dropdowns
            viewModel.StatusOptions = await _statusService.GetStatusAllActiveSelectListAsync();


            // Checkbox Groups
            viewModel.ImpactedPersonTypeOptions = await _impactedPersonTypeService.GetAllActiveSelectListAsync();
            viewModel.IncidentBehaviourTypeOptions = await _incidentBehaviourTypeService.GetAllActiveSelectListAsync();
            viewModel.IncidentMotivationTypeOptions = await _incidentMotivationTypeService.GetAllActiveSelectListAsync();

            return viewModel;
        }


        // Method to map ViewModel to EntityModel
        public MainFormEntityModel MapViewToEntity(MainFormViewModel viewModel)
        {
            return MapSharedFieldsViewToEntity(viewModel, new MainFormEntityModel());
        }

        public MainFormAdminEntityModel MapViewToEntityAdmin(MainFormAdminViewModel viewModel)
        {
            var entity = MapSharedFieldsViewToEntity(viewModel, new MainFormAdminEntityModel());

            // Textboxes
            entity.StaffMemberAssignedAdmin = _inputSanitizer.Sanitize(viewModel.StaffMemberAssignedAdmin);


            // Textareas
            entity.ActionTakenByCollegeAdmin = _inputSanitizer.Sanitize(viewModel.ActionTakenByCollegeAdmin);
            entity.AdminNote = _inputSanitizer.Sanitize(viewModel.AdminNote);


            // Radios


            // Dropdown Lists


            // Checkboxes


            return entity;
        }




        /// <summary>
        /// Maps common/shared fields from a view model to an entity model. 
        /// Intended for use when saving form data from the UI to the database.
        /// </summary>
        /// <typeparam name="TEntity">The entity model type (must inherit from MainFormEntityModel).</typeparam>
        /// <typeparam name="TViewModel">The view model type (must inherit from MainFormViewModel).</typeparam>
        /// <param name="viewModel">The source view model containing user input.</param>
        /// <param name="entity">The destination entity model to populate with view model data.</param>
        /// <returns>The updated entity model.</returns>
        private TEntity MapSharedFieldsViewToEntity<TEntity, TViewModel>(TViewModel viewModel, TEntity entity)
                where TEntity : MainFormEntityModel
                where TViewModel : MainFormViewModel
        {
            // Basic identification and tracking fields
            entity.Id = viewModel.Id;
            entity.SubmittedByWindowsUserName = viewModel.SubmittedByWindowsUserName;

            // Textboxes (input sanitization to prevent injection or formatting issues)
            entity.StaffFullName = _inputSanitizer.Sanitize(viewModel.StaffFullName);
            entity.StaffTelephoneNumber = _inputSanitizer.Sanitize(viewModel.StaffTelephoneNumber);
            entity.StaffEmail = viewModel.StaffEmail;
            entity.IncidentPersonName = _inputSanitizer.Sanitize(viewModel.IncidentPersonName);
            entity.IncidentDate = viewModel.IncidentDate;

            // Textareas
            entity.IncidentDetails = _inputSanitizer.Sanitize(viewModel.IncidentDetails);


            // Radios (throw if not selected — these are required fields)
            entity.IncidentHappenedToId = viewModel.SelectedIncidentHappenedToId ?? throw new InvalidOperationException("IncidentHappenedToId is required");
            entity.NumberOfPeopleImpactedId = viewModel.SelectedNumberOfPeopleImpactedId ?? throw new InvalidOperationException("NumberOfPeopleImpactedId is required");
            entity.NumberOfPeopleCausedIncidentId = viewModel.SelectedNumberOfPeopleCausedIncidentId ?? throw new InvalidOperationException("NumberOfPeopleCausedIncidentId is required");
            entity.IncidentLocationId = viewModel.SelectedIncidentLocationId ?? throw new InvalidOperationException("IncidentLocationId is required");
            entity.HasSimilarIncidentHappenedBeforeId = viewModel.SelectedHasSimilarIncidentHappenedBeforeId ?? throw new InvalidOperationException("HasSimilarIncidentHappenedBeforeId is required");


            // Dropdown Lists


            // Checkboxes (stored as list of selected IDs)
            entity.SelectedImpactedPersonTypeIds = viewModel.SelectedImpactedPersonTypeIds;
            entity.SelectedIncidentBehaviourTypeIds = viewModel.SelectedIncidentBehaviourTypeIds;
            entity.SelectedIncidentMotivationTypeIds = viewModel.SelectedIncidentMotivationTypeIds;


            // Predefined dropdown (e.g. form status)
            entity.StatusId = viewModel.StatusId;

            return entity;
        }




        // Public method for regular ViewModel mapping
        public async Task<MainFormViewModel> MapEntityToView(MainFormEntityModel entityModel, ViewModelMappingContext context)
        {
            var vm = await MapSharedFieldsEntityToView(entityModel, context, new MainFormViewModel());
            return vm;
        }



        // Public method for admin ViewModel mapping
        public async Task<MainFormAdminViewModel> MapEntityToViewAdmin(MainFormAdminEntityModel entityModel, 
                                                                        AdminViewModelMappingContext context)
        {
            var vm = await MapSharedFieldsEntityToView(entityModel, context, new MainFormAdminViewModel());


            // Textboxes

            vm.StaffFullName = _inputSanitizer.Sanitize(entityModel.StaffFullName);
            vm.StaffTelephoneNumber = _inputSanitizer.Sanitize(entityModel.StaffTelephoneNumber);
            vm.StaffEmail = entityModel.StaffEmail;
            vm.IncidentPersonName = _inputSanitizer.Sanitize(entityModel.IncidentPersonName);
            vm.IncidentDate = entityModel.IncidentDate;

            vm.StaffMemberAssignedAdmin = _inputSanitizer.Sanitize(entityModel.StaffMemberAssignedAdmin);



            // Textareas
            vm.ActionTakenByCollegeAdmin = _inputSanitizer.Sanitize(entityModel.ActionTakenByCollegeAdmin);
            vm.AdminNote = _inputSanitizer.Sanitize(entityModel.AdminNote);


            // Radios


            // Dropdown Lists
            vm.AdminStatusId = entityModel.StatusId;





            // Checkboxes
            // Checkbox group selections (stored as list of selected IDs)
            // Selected xyz Ids come directly from the entity model.
            // These represent the raw, persisted user selections (domain data),
            // and should be mapped as-is during the entity-to-view model conversion.


            vm.AdminAuditFormatted = context.AuditFormattedHtml;

            return vm;
        }


        /// <summary>
        /// Shared logic to map common fields from entity + context into a ViewModel.
        /// Generic over TEntity (entity model) and TViewModel (view model).
        /// </summary>
        private async Task<TViewModel> MapSharedFieldsEntityToView<TEntity, TViewModel>(
                                                                            TEntity entityModel,
                                                                            dynamic context,  // context type varies by usage, so dynamic or interface
                                                                            TViewModel vm)
            where TEntity : MainFormEntityModel
            where TViewModel : MainFormViewModel
        {
            vm.Id = entityModel.Id;
            vm.SubmittedByWindowsUserName = entityModel.SubmittedByWindowsUserName;

            // Textboxes (input sanitization to prevent injection or formatting issues)
            vm.StaffFullName = _inputSanitizer.Sanitize(entityModel.StaffFullName);
            vm.StaffTelephoneNumber = _inputSanitizer.Sanitize(entityModel.StaffTelephoneNumber);
            vm.StaffEmail = entityModel.StaffEmail;
            vm.IncidentPersonName = _inputSanitizer.Sanitize(entityModel.IncidentPersonName);
            vm.IncidentDate = entityModel.IncidentDate;

            // Textareas
            vm.IncidentDetails = _inputSanitizer.Sanitize(entityModel.IncidentDetails);


            // Radios
            vm.SelectedIncidentHappenedToId = entityModel.IncidentHappenedToId;
            vm.IncidentHappenedToName = context.SelectedIncidentHappenedToName;
            vm.IncidentHappenedToOptions = context.IncidentHappenedToOptions;

            vm.SelectedNumberOfPeopleImpactedId = entityModel.NumberOfPeopleImpactedId;
            vm.NumberOfPeopleImpactedName = context.SelectedNumberOfPeopleImpactedName;
            vm.NumberOfPeopleImpactedOptions = context.NumberOfPeopleImpactedOptions;

            vm.SelectedNumberOfPeopleCausedIncidentId = entityModel.NumberOfPeopleCausedIncidentId;
            vm.NumberOfPeopleCausedIncidentName = context.SelectedNumberOfPeopleCausedIncidentName;
            vm.NumberOfPeopleCausedIncidentOptions = context.NumberOfPeopleCausedIncidentOptions;

            vm.SelectedIncidentLocationId = entityModel.IncidentLocationId;
            vm.IncidentLocationName = context.SelectedIncidentLocationName;
            vm.IncidentLocationOptions = context.IncidentLocationOptions;

            vm.SelectedHasSimilarIncidentHappenedBeforeId = entityModel.HasSimilarIncidentHappenedBeforeId;
            vm.HasSimilarIncidentHappenedBeforeName = context.SelectedHasSimilarIncidentHappenedBeforeName;
            vm.HasSimilarIncidentHappenedBeforeOptions = context.HasSimilarIncidentHappenedBeforeOptions;


            // Dropdowns




            // Checkboxes

            // SelectedSampleCheckboxIds come directly from the entity model.
            // These represent the raw, persisted user selections (domain data),
            // and should be mapped as-is during the entity-to-view model conversion.
            vm.SelectedImpactedPersonTypeIds = await _mainFormImpactedPersonTypeService.GetSelectedImpactedPersonTypeIdsByMainFormIdAsync(entityModel.Id);
            // ImpactedPersonTypeOptions are loaded from the lookup service and provided by the context.
            // These are used to populate the checkbox list in the UI and are not stored in the entity.
            vm.ImpactedPersonTypeOptions = context.ImpactedPersonTypeOptions;
            // SelectedImpactedPersonTypeNames are derived by matching the selected IDs against the lookup options.
            // This is purely for display purposes (e.g., showing a summary of selected names) and is also provided by the context.
            vm.SelectedImpactedPersonTypeNames = context.SelectedImpactedPersonTypeNames;



            vm.SelectedIncidentBehaviourTypeIds = await _mainFormIncidentBehaviourTypeService.GetSelectedIncidentBehaviourTypeIdsByMainFormIdAsync(entityModel.Id);
            vm.IncidentBehaviourTypeOptions = context.IncidentBehaviourTypeOptions;
            vm.SelectedIncidentBehaviourTypeNames = context.SelectedIncidentBehaviourTypeNames;

            vm.SelectedIncidentMotivationTypeIds = await _mainFormIncidentMotivationTypeService.GetSelectedIncidentMotivationTypeIdsByMainFormIdAsync(entityModel.Id);
            vm.IncidentMotivationTypeOptions = context.IncidentMotivationTypeOptions;
            vm.SelectedIncidentMotivationTypeNames = context.SelectedIncidentMotivationTypeNames;



            // Predefined
            vm.DateSubmitted = entityModel.DateSubmitted;
            vm.StatusId = entityModel.StatusId;
            var statusOptions = (List<SelectListItem>)context.StatusOptions;
            vm.StatusDisplayName = statusOptions
                .FirstOrDefault(s => s.Value == entityModel.StatusId.ToString())?.Text ?? "Unknown";

            return vm;
        }





        /// <summary>
        /// Resolves and populates human-readable display names for selected values in the view model.
        /// 
        /// This method is useful for rendering details, summary views, or emails where the UI
        /// should show the friendly names of selected options rather than raw IDs.
        /// 
        /// The method handles both dropdowns and checkbox groups:
        /// - For dropdowns, it finds the corresponding <see cref="Microsoft.AspNetCore.Mvc.Rendering.SelectListItem.Text"/>
        ///   based on the selected ID and assigns it to a display property.
        /// - For checkbox groups, it filters the selected IDs and collects their corresponding names.
        /// </summary>
        /// <param name="viewModel">The <see cref="MainFormViewModel"/> containing selected IDs and option lists.</param>

        public void PopulateDisplayProperties(MainFormViewModel viewModel)
        {
            // Dropdowns

            // Status dropdown: If options exist, find the text matching the selected StatusId
            if (viewModel.StatusOptions?.Any() == true)
            {
                viewModel.StatusDisplayName = viewModel.StatusOptions
                    .FirstOrDefault(x => x.Value == viewModel.StatusId?.ToString())?.Text
                    ?? string.Empty;
            }

            // Checkboxes
            // ImpactedPersonType: If options exist, find all names corresponding to selected checkbox IDs
            if (viewModel.ImpactedPersonTypeOptions?.Any() == true)
            {
                viewModel.SelectedImpactedPersonTypeNames = viewModel.ImpactedPersonTypeOptions
                    .Where(x => viewModel.SelectedImpactedPersonTypeIds.Contains(int.Parse(x.Value)))
                    .Select(x => x.Text)
                    .ToList();
            }

            if (viewModel.IncidentBehaviourTypeOptions?.Any() == true)
            {
                viewModel.SelectedIncidentBehaviourTypeNames = viewModel.IncidentBehaviourTypeOptions
                    .Where(x => viewModel.SelectedIncidentBehaviourTypeIds.Contains(int.Parse(x.Value)))
                    .Select(x => x.Text)
                    .ToList();
            }

            if (viewModel.IncidentMotivationTypeOptions?.Any() == true)
            {
                viewModel.SelectedIncidentMotivationTypeNames = viewModel.IncidentMotivationTypeOptions
                    .Where(x => viewModel.SelectedIncidentMotivationTypeIds.Contains(int.Parse(x.Value)))
                    .Select(x => x.Text)
                    .ToList();
            }
            


        }



        /// <summary>
        /// Populates human‑friendly display properties on the admin view model
        /// based on the available select‑list options and the selected IDs.
        /// </summary>
        public void PopulateDisplayPropertiesAdmin(MainFormAdminViewModel viewModel)
        {
            // Dropdowns




            // If status options are provided, find and set the display name matching the selected StatusId
            if (viewModel.StatusOptions?.Any() == true)
            {
                viewModel.StatusDisplayName = viewModel.StatusOptions
                    .FirstOrDefault(x => x.Value == viewModel.StatusId?.ToString())?.Text
                    ?? string.Empty;
            }



            // Checkboxes

            // ImpactedPersonType: If options exist, find all names corresponding to selected checkbox IDs
            if (viewModel.ImpactedPersonTypeOptions?.Any() == true)
            {
                viewModel.SelectedImpactedPersonTypeNames = viewModel.ImpactedPersonTypeOptions
                    .Where(x => viewModel.SelectedImpactedPersonTypeIds.Contains(int.Parse(x.Value)))
                    .Select(x => x.Text)
                    .ToList();
            }

            if (viewModel.IncidentBehaviourTypeOptions?.Any() == true)
            {
                viewModel.SelectedIncidentBehaviourTypeNames = viewModel.IncidentBehaviourTypeOptions
                    .Where(x => viewModel.SelectedIncidentBehaviourTypeIds.Contains(int.Parse(x.Value)))
                    .Select(x => x.Text)
                    .ToList();
            }

            if (viewModel.IncidentMotivationTypeOptions?.Any() == true)
            {
                viewModel.SelectedIncidentMotivationTypeNames = viewModel.IncidentMotivationTypeOptions
                    .Where(x => viewModel.SelectedIncidentMotivationTypeIds.Contains(int.Parse(x.Value)))
                    .Select(x => x.Text)
                    .ToList();
            }

        }









    }
}
