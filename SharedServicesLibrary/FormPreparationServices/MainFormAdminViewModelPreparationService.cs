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
    public class MainFormAdminViewModelPreparationService : IMainFormAdminViewModelPreparationService
    {
        private readonly ISampleCheckboxService _sampleCheckboxService;
        private readonly IStatusService _statusService;
        private readonly IMainFormAdminService _mainFormAdminService;
        private readonly IMainFormSampleCheckboxService _mainFormSampleCheckboxService;
        private readonly IFormHandlingService _formHandlingService;
        private readonly IAdminViewModelContextBuilder _adminViewModelContextBuilder;

        public MainFormAdminViewModelPreparationService(ISampleCheckboxService sampleCheckboxService,
                                      IStatusService statusService,
                                      IMainFormAdminService mainFormAdminService,
                                      IMainFormSampleCheckboxService mainFormSampleCheckboxService,
                                      IFormHandlingService formHandlingService,
                                      IAdminViewModelContextBuilder adminViewModelContextBuilder)

        {
            _sampleCheckboxService = sampleCheckboxService;
            _statusService = statusService;
            _mainFormAdminService = mainFormAdminService;
            _mainFormSampleCheckboxService = mainFormSampleCheckboxService;
            _formHandlingService = formHandlingService;
            _adminViewModelContextBuilder = adminViewModelContextBuilder;
        }




        public async Task<PagedResult<MainFormAdminViewModel>> PreparePagedListAdminViewModelAsync(
                                                        string? studentReferenceNumber,
                                                        int? statusId,
                                                        string? sortBy,
                                                        string? sortDirection,
                                                        int? pageNumber,
                                                        int? pageSize)
        {
            // Step 1: Get paged items and total count from the same stored procedure call
            var (submissionEntityList, totalRecords) = await _mainFormAdminService.GetAllOrByFilterAsync(
                studentReferenceNumber, statusId, sortBy, sortDirection, pageNumber, pageSize);

            var viewModelList = new List<MainFormAdminViewModel>();

            // Step 2: Map each entity to its enriched ViewModel
            foreach (var submission in submissionEntityList)
            {
                var context = await _adminViewModelContextBuilder.BuildAsync(submission.Id);
                var mainFormAdminViewModel = await _formHandlingService.MapEntityToViewAdmin(submission, context);
                viewModelList.Add(mainFormAdminViewModel);
            }

            // Step 3: Return paged result
            return new PagedResult<MainFormAdminViewModel>
            {
                Items = viewModelList,
                PageNumber = pageNumber ?? 1,
                PageSize = pageSize ?? 50,
                TotalRecords = totalRecords
            };
        }



        public async Task<MainFormAdminViewModel> PreparePopulatedAdminViewModelAsync(int id)
        {
            // Retrieve the entity model for the given id
            var mainFormAdminEntityModel = await _mainFormAdminService.GetById(id);

            if (mainFormAdminEntityModel == null)
            {
                // Return null to indicate the entity was not found.
                // The calling method should check for null and perform
                // appropriate handling, such as redirecting to a NotFound page.
                return null;
            }


            // Use the AdminViewModelContextBuilder to build the common context for the admin view model
            // The context contains additional data required for the view model that is not present in the entity model.
            // Specifically, it includes:
            // - Selected feature, film, and status IDs, as well as their corresponding human-readable names (e.g., selectedFeatureNames, selectedFilmNames, selectedStatusName)
            // - Options for dropdowns and other UI elements (e.g., featureOptions, filmOptions, statusOptions)
            // - Audit informationfrom tblAuditLog
            // The entity model represents raw submission data, but the context enriches the view model with related data 
            // such as name mappings and UI options needed to display a complete and user-friendly view to the admin user.
            var uiContext = await _adminViewModelContextBuilder.BuildAsync(id);

            // Map the entity to the view model using the built context
            MainFormAdminViewModel mainFormAdminViewModel = await _formHandlingService.MapEntityToViewAdmin(mainFormAdminEntityModel, uiContext);

            // Populate any admin-specific dropdowns or additional lists
            mainFormAdminViewModel = await _formHandlingService.PopulateDropdownsAndListsAdmin(mainFormAdminViewModel);

            return mainFormAdminViewModel;
        }









    }
}
