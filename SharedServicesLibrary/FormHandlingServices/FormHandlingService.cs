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
        private readonly ISampleCheckboxService _sampleCheckboxService;
        private readonly IStatusService _statusService;
        private readonly IFileUploadHelper _fileUploadHelper;
        private readonly IInputSanitizer _inputSanitizer;
        private readonly ISampleDropdownService _sampleDropdownService;
        private readonly IMainFormSampleCheckboxService _mainFormSampleCheckboxService;
        private readonly ISampleRadioService _sampleRadioService;
        private readonly ISampleRadioAdminService _sampleRadioAdminService;
        private readonly IMainFormSampleCheckboxAdminService _mainFormSampleCheckboxAdminService;
        private readonly ISampleCheckboxAdminService _sampleCheckboxAdminService;
        private readonly ISampleDropdownAdminService _sampleDropdownAdminService;

        public FormHandlingService(ISampleCheckboxService sampleCheckboxService,
                                   ISampleCheckboxAdminService sampleCheckboxAdminService,
                                   IStatusService statusService,
                                   IFileUploadHelper fileUploadHelper,
                                   IInputSanitizer inputSanitizer,
                                   ISampleDropdownService sampleDropdownService,
                                   ISampleDropdownAdminService sampleDropdownAdminService,
                                   IMainFormSampleCheckboxService mainFormSampleCheckboxService,
                                   ISampleRadioService sampleRadioService,
                                   ISampleRadioAdminService sampleRadioAdminService,
                                   IMainFormSampleCheckboxAdminService mainFormSampleCheckboxAdminService
                                   )
        {
            _sampleCheckboxService = sampleCheckboxService;
            _statusService = statusService;
            _fileUploadHelper = fileUploadHelper;
            _inputSanitizer = inputSanitizer;
            _sampleDropdownService = sampleDropdownService;
            _mainFormSampleCheckboxService = mainFormSampleCheckboxService;
            _sampleRadioService = sampleRadioService;
            _sampleRadioAdminService = sampleRadioAdminService;
            _mainFormSampleCheckboxAdminService = mainFormSampleCheckboxAdminService;
            _sampleCheckboxAdminService = sampleCheckboxAdminService;
            _sampleDropdownAdminService = sampleDropdownAdminService;
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
            viewModel.SampleRadioOptions = await _sampleRadioService.GetAllActiveSelectListAsync();


            // DropDownlist
            viewModel.SampleDropdownOptions = await _sampleDropdownService.GetAllActiveSelectListAsync();


            // If an ID is selected, attempt to resolve its corresponding display name
            // by searching the associated SelectList.
            // This allows the application to display a human-readable label (e.g., on Details views or in emails)
            // instead of just showing the underlying ID value.
            if (viewModel.SelectedSampleRadioId.HasValue)
            {
                // Find the matching SelectListItem using the selected ID (converted to string for comparison),
                // as SelectListItem.Value is always a string.
                var selectedSampleRadio = viewModel.SampleRadioOptions
                    .FirstOrDefault(option => option.Value == viewModel.SelectedSampleRadioId.Value.ToString());

                // If a matching item is found, assign its display text to ItemName.
                // This is used in Details views to show the human-readable item name.
                viewModel.SampleRadioName = selectedSampleRadio?.Text;
            }



            // Checkbox group
            // Because the selected values are processed and displayed directly in the view
            // by comparing Selected[Entity]Ids to the SelectListItems, there is no need
            // to separately resolve and store the human-readable names on the server.
            viewModel.SampleCheckboxOptions = await _sampleCheckboxService.GetAllActiveSelectListAsync();



            // Predefined
            viewModel.StatusOptions = await _statusService.GetStatusAllActiveSelectListAsync();

            return viewModel;
        }

        public async Task<MainFormAdminViewModel> PopulateDropdownsAndListsAdmin(MainFormAdminViewModel viewModel)
        {
            // Radios
            viewModel.SampleRadioOptions = await _sampleRadioService.GetAllActiveSelectListAsync();
            viewModel.SampleRadioAdminOptions = await _sampleRadioAdminService.GetAllActiveSelectListAsync();


            // Dropdowns
            viewModel.SampleDropdownOptions = await _sampleDropdownService.GetAllActiveSelectListAsync();
            viewModel.SampleDropdownAdminOptions = await _sampleDropdownAdminService.GetAllActiveSelectListAsync();
            viewModel.StatusOptions = await _statusService.GetStatusAllActiveSelectListAsync();


            // Checkbox Groups
            viewModel.SampleCheckboxOptions = await _sampleCheckboxService.GetAllActiveSelectListAsync();
            viewModel.SampleCheckboxAdminOptions = await _sampleCheckboxAdminService.GetAllActiveSelectListAsync();


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
            entity.SampleTextboxAdmin = _inputSanitizer.Sanitize(viewModel.SampleTextboxAdmin);
            entity.SampleDateAdmin = viewModel.SampleDateAdmin;
            entity.SampleCostAdmin = viewModel.SampleCostAdmin;


            // Textareas
            entity.AdminNote = _inputSanitizer.Sanitize(viewModel.AdminNote);
            entity.SampleTextareaAdmin = _inputSanitizer.Sanitize(viewModel.SampleTextareaAdmin);


            // Radios
            entity.SampleRadioAdminId = viewModel.SelectedSampleRadioAdminId
                                                    ?? throw new InvalidOperationException("SampleRadioadminId is required");


            // Dropdown Lists
            entity.SampleDropdownAdminId = viewModel.SampleDropdownAdminId;



            // Checkboxes
            entity.SelectedSampleCheckboxAdminIds = viewModel.SelectedSampleCheckboxAdminIds;


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
            entity.StudentReferenceNumber = viewModel.StudentReferenceNumber?.ToUpperInvariant(); // Ensure consistent casing
            entity.StudentDateOfBirth = viewModel.StudentDateOfBirth;
            entity.SubmittedByWindowsUserName = viewModel.SubmittedByWindowsUserName;

            // Textboxes (input sanitization to prevent injection or formatting issues)
            entity.StudentFullName = _inputSanitizer.Sanitize(viewModel.StudentFullName);

            entity.StaffFullName = _inputSanitizer.Sanitize(viewModel.StaffFullName);
            entity.StaffTelephoneNumber = _inputSanitizer.Sanitize(viewModel.StaffTelephoneNumber);
            entity.StaffEmail = _inputSanitizer.Sanitize(viewModel.StaffEmail);
            entity.IncidentPersonName = _inputSanitizer.Sanitize(viewModel.IncidentPersonName);
            entity.IncidentDate = viewModel.IncidentDate;


            entity.SampleTextbox = _inputSanitizer.Sanitize(viewModel.SampleTextbox);
            entity.SampleDate = viewModel.SampleDate;
            entity.SampleTime = viewModel.SampleTime;
            entity.SampleCost = viewModel.SampleCost;


            // Textareas
            entity.SampleTextarea = _inputSanitizer.Sanitize(viewModel.SampleTextarea);


            // Radios (throw if not selected — these are required fields)
            entity.SampleRadioId = viewModel.SelectedSampleRadioId
                ?? throw new InvalidOperationException("SampleRadioId is required");


            // Dropdown Lists
            entity.SampleDropdownId = viewModel.SampleDropdownId;


            // Checkboxes (stored as list of selected IDs)
            entity.SelectedSampleCheckboxIds = viewModel.SelectedSampleCheckboxIds;



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
            vm.SampleTextboxAdmin = _inputSanitizer.Sanitize(entityModel.SampleTextboxAdmin);
            vm.SampleDateAdmin = entityModel.SampleDateAdmin;
            vm.SampleCostAdmin = entityModel.SampleCostAdmin;

            vm.StaffFullName = _inputSanitizer.Sanitize(entityModel.StaffFullName);
            vm.StaffTelephoneNumber = _inputSanitizer.Sanitize(entityModel.StaffTelephoneNumber);
            vm.StaffEmail = _inputSanitizer.Sanitize(entityModel.StaffEmail);
            vm.IncidentPersonName = _inputSanitizer.Sanitize(entityModel.IncidentPersonName);
            vm.IncidentDate = entityModel.IncidentDate;




            // Textareas
            vm.AdminNote = _inputSanitizer.Sanitize(entityModel.AdminNote);
            vm.SampleTextareaAdmin = _inputSanitizer.Sanitize(entityModel.SampleTextareaAdmin);


            // Radios
            vm.SelectedSampleRadioAdminId = entityModel.SampleRadioAdminId;
            vm.SampleRadioAdminName = context.SelectedSampleRadioAdminName;


            // Dropdown Lists
            vm.AdminStatusId = entityModel.StatusId;


            var sampleDropdownAdminId = entityModel.SampleDropdownAdminId;
            var sampleDropdownAdminName = context.SampleDropdownAdminOptions
                                .FirstOrDefault(s => s.Value == sampleDropdownAdminId.ToString())?.Text
                             ?? "-";
            vm.SampleDropdownAdminId = sampleDropdownAdminId;
            vm.SampleDropdownAdminDisplayName = sampleDropdownAdminName;





            // Checkboxes
            // Checkbox group selections (stored as list of selected IDs)
            // Selected xyz Ids come directly from the entity model.
            // These represent the raw, persisted user selections (domain data),
            // and should be mapped as-is during the entity-to-view model conversion.
            vm.SelectedSampleCheckboxAdminIds = await _mainFormSampleCheckboxAdminService.GetSelectedSampleCheckboxAdminIdsByMainFormIdAsync(entityModel.Id);
            vm.SampleCheckboxAdminOptions = context.SampleCheckboxAdminOptions;
            vm.SelectedSampleCheckboxAdminNames = context.SelectedSampleCheckboxAdminNames;



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
            vm.StudentReferenceNumber = entityModel.StudentReferenceNumber;
            vm.StudentDateOfBirth = entityModel.StudentDateOfBirth;
            vm.SubmittedByWindowsUserName = entityModel.SubmittedByWindowsUserName;

            // Textboxes (input sanitization to prevent injection or formatting issues)
            vm.StudentFullName = _inputSanitizer.Sanitize(entityModel.StudentFullName);

            vm.StaffFullName = _inputSanitizer.Sanitize(entityModel.StaffFullName);
            vm.StaffTelephoneNumber = _inputSanitizer.Sanitize(entityModel.StaffTelephoneNumber);
            vm.StaffEmail = _inputSanitizer.Sanitize(entityModel.StaffEmail);
            vm.IncidentPersonName = _inputSanitizer.Sanitize(entityModel.IncidentPersonName);
            vm.IncidentDate = entityModel.IncidentDate;



            vm.SampleTextbox = _inputSanitizer.Sanitize(entityModel.SampleTextbox);
            vm.SampleDate = entityModel.SampleDate;
            vm.SampleTime = entityModel.SampleTime;
            vm.SampleCost = entityModel.SampleCost;

            // Textareas
            vm.SampleTextarea = _inputSanitizer.Sanitize(entityModel.SampleTextarea);


            // Radios
            vm.SelectedSampleRadioId = entityModel.SampleRadioId;
            vm.SampleRadioName = context.SelectedSampleRadioName;
            vm.SampleRadioOptions = context.SampleRadioOptions;


            // Dropdowns
            vm.SampleDropdownId = entityModel.SampleDropdownId;
            var sampleDropdownOptions = (List<SelectListItem>)context.SampleDropdownOptions;
            vm.SampleDropdownOptions = sampleDropdownOptions;
            var sampleDropdownName = sampleDropdownOptions
                .FirstOrDefault(s => s.Value == entityModel.SampleDropdownId.ToString())?.Text ?? "Unknown";
            vm.SampleDropdownDisplayName = sampleDropdownName;





            // Checkboxes

            // SelectedSampleCheckboxIds come directly from the entity model.
            // These represent the raw, persisted user selections (domain data),
            // and should be mapped as-is during the entity-to-view model conversion.
            vm.SelectedSampleCheckboxIds = await _mainFormSampleCheckboxService.GetSelectedSampleCheckboxIdsByMainFormIdAsync(entityModel.Id);
            // SampleCheckboxOptions are loaded from the lookup service and provided by the context.
            // These are used to populate the checkbox list in the UI and are not stored in the entity.
            vm.SampleCheckboxOptions = context.SampleCheckboxOptions;
            // SelectedSampleCheckboxNames are derived by matching the selected IDs against the lookup options.
            // This is purely for display purposes (e.g., showing a summary of selected names) and is also provided by the context.
            vm.SelectedSampleCheckboxNames = context.SelectedSampleCheckboxNames;



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
            // SampleDropdown: If options exist, find the text matching the selected SampleDropdownId
            if (viewModel.SampleDropdownOptions?.Any() == true)
            {
                viewModel.SampleDropdownDisplayName = viewModel.SampleDropdownOptions
                    .FirstOrDefault(x => x.Value == viewModel.SampleDropdownId?.ToString())?.Text
                    ?? string.Empty;  // Fallback to empty if not found
            }


            // Status dropdown: If options exist, find the text matching the selected StatusId
            if (viewModel.StatusOptions?.Any() == true)
            {
                viewModel.StatusDisplayName = viewModel.StatusOptions
                    .FirstOrDefault(x => x.Value == viewModel.StatusId?.ToString())?.Text
                    ?? string.Empty;
            }

            // Checkboxes
            // SampleCheckbox: If options exist, find all names corresponding to selected checkbox IDs
            if (viewModel.SampleCheckboxOptions?.Any() == true)
            {
                viewModel.SelectedSampleCheckboxNames = viewModel.SampleCheckboxOptions
                    .Where(x => viewModel.SelectedSampleCheckboxIds.Contains(int.Parse(x.Value)))
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

            // If SampleDropdown options are provided, find and set the display name matching the selected GenderId
            if (viewModel.SampleDropdownOptions?.Any() == true)
            {
                viewModel.SampleDropdownDisplayName = viewModel.SampleDropdownOptions
                    .FirstOrDefault(x => x.Value == viewModel.SampleDropdownId?.ToString())?.Text
                    ?? string.Empty;  // Fallback to empty if not found
            }

            if (viewModel.SampleDropdownAdminOptions?.Any() == true)
            {
                viewModel.SampleDropdownAdminDisplayName = viewModel.SampleDropdownAdminOptions
                    .FirstOrDefault(x => x.Value == viewModel.SampleDropdownAdminId?.ToString())?.Text
                    ?? string.Empty;  // Fallback to empty if not found
            }




            // If status options are provided, find and set the display name matching the selected StatusId
            if (viewModel.StatusOptions?.Any() == true)
            {
                viewModel.StatusDisplayName = viewModel.StatusOptions
                    .FirstOrDefault(x => x.Value == viewModel.StatusId?.ToString())?.Text
                    ?? string.Empty;
            }



            // Checkboxes

            // If SampleCheckbox options exist, filter by the selected IDs and collect their names
            if (viewModel.SampleCheckboxOptions?.Any() == true)
            {
                viewModel.SelectedSampleCheckboxNames = viewModel.SampleCheckboxOptions
                    .Where(x => viewModel.SelectedSampleCheckboxIds.Contains(int.Parse(x.Value)))
                    .Select(x => x.Text)
                    .ToList();
            }

            if (viewModel.SampleCheckboxAdminOptions?.Any() == true)
            {
                viewModel.SelectedSampleCheckboxAdminNames = viewModel.SampleCheckboxAdminOptions
                    .Where(x => viewModel.SelectedSampleCheckboxAdminIds.Contains(int.Parse(x.Value)))
                    .Select(x => x.Text)
                    .ToList();
            }

        }









    }
}
