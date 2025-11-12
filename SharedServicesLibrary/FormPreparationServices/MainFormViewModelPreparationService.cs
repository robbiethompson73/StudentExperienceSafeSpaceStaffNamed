using AyrshireCollege.Biis.UserIdentityLibrary.IdentityServices;
using DataAccessLibrary.DataServices;
using DataAccessLibrary.Models;
using SharedServicesLibrary.FormHandlingServices;
using SharedServicesLibrary.FormPreparationModels;
using SharedViewModelLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServicesLibrary.FormPreparationServices
{
    public class MainFormViewModelPreparationService : IMainFormViewModelPreparationService
    {
        private readonly ISampleCheckboxService _sampleCheckboxService;
        private readonly IStatusService _statusService;
        private readonly IMainFormService _mainFormService;
        private readonly IMainFormSampleCheckboxService _mainFormSampleCheckboxService;
        private readonly IFormHandlingService _formHandlingService;
        private readonly IViewModelContextBuilder _viewModelContextBuilder;
        private readonly IIdentityService _identityService;

        public MainFormViewModelPreparationService(ISampleCheckboxService sampleCheckboxService,
                                      IStatusService statusService,
                                      IMainFormService mainFormService,
                                      IMainFormSampleCheckboxService mainFormSampleCheckboxService,
                                      IFormHandlingService formHandlingService,
                                      IViewModelContextBuilder viewModelContextBuilder,
                                      IIdentityService identityService)
        {
            _sampleCheckboxService = sampleCheckboxService;
            _statusService = statusService;
            _mainFormService = mainFormService;
            _mainFormSampleCheckboxService = mainFormSampleCheckboxService;
            _formHandlingService = formHandlingService;
            _viewModelContextBuilder = viewModelContextBuilder;
            _identityService = identityService;
        }


        public async Task<MainFormViewModel> PrepareCreateViewModelAsync()
        {
            MainFormViewModel mainFormViewModel = new MainFormViewModel();

            return await _formHandlingService.PopulateDropdownsAndListsStaff(mainFormViewModel);
        }


        public async Task<List<MainFormViewModel>> PrepareListViewModelAsync()
        {
            var windowsName = await _identityService.GetUnformattedUserNameAsync();

            var submissionEntityList = await _mainFormService.GetBySubmittedByWindowsUserName(windowsName);

            var viewModelList = new List<MainFormViewModel>();

            foreach (var entitySubmission in submissionEntityList)
            {
                // Map the entity to the view model using the built uiContext
                // The uiContext contains additional data required for the view model not present in the entity model.
                // Specifically, it includes:
                // - Selected feature, film, and status IDs, as well as their corresponding human-readable names (e.g., selectedFeatureNames, selectedFilmNames, selectedStatusName)
                // - Options for dropdowns and other UI elements (e.g., featureOptions, filmOptions, statusOptions)
                // The entity model represents raw submission data, but the uiContext enriches the view model with related data 
                // such as name mappings and UI options needed to display a complete and user-friendly view to the admin user.
                // context = "data needed to construct something"
                var uiContext = await _viewModelContextBuilder.BuildAsync(entitySubmission.Id);

                // Use MapEntityModelToView to map the EntityModel to the ViewModel with the created context
                MainFormViewModel mainFormViewModel = await _formHandlingService.MapEntityToView(entitySubmission, uiContext);

                // Add the view model to the list
                viewModelList.Add(mainFormViewModel);
            }

            return viewModelList;

        }



        public async Task<MainFormViewModel> PreparePopulatedViewModelAsync(int id)
        {
            // Retrieve the entity model for the given id
            var mainFormEntityModel = await _mainFormService.GetById(id);

            // Map the entity to the view model using the built uiContext
            // The uiContext contains additional data required for the view model not present in the entity model.
            // Specifically, it includes:
            // - Selected feature, film, and status IDs, as well as their corresponding human-readable names (e.g., selectedFeatureNames, selectedFilmNames, selectedStatusName)
            // - Options for dropdowns and other UI elements (e.g., featureOptions, filmOptions, statusOptions)
            // The entity model represents raw submission data, but the uiContext enriches the view model with related data 
            // such as name mappings and UI options needed to display a complete and user-friendly view to the admin user.
            // context = "data needed to construct something"
            var uiContext = await _viewModelContextBuilder.BuildAsync(id);

            // Use MapEntityModelToView to map the EntityModel to the ViewModel
            MainFormViewModel mainFormViewModel = await _formHandlingService.MapEntityToView(mainFormEntityModel, uiContext);

            return mainFormViewModel;
        }









    }
}
