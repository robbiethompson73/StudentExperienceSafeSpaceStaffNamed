using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SharedServicesLibrary.FormPreparationModels;
using SharedViewModelLibrary.Models;
using System.ComponentModel.DataAnnotations;

namespace SharedServicesLibrary.FormHandlingServices
{
    public interface IFormHandlingService
    {
        Task<MainFormViewModel> MapEntityToView(MainFormEntityModel entityModel, ViewModelMappingContext context);
        Task<MainFormAdminViewModel> MapEntityToViewAdmin(MainFormAdminEntityModel entityModel, AdminViewModelMappingContext context);
        MainFormEntityModel MapViewToEntity(MainFormViewModel viewModel);
        MainFormAdminEntityModel MapViewToEntityAdmin(MainFormAdminViewModel viewModel);
        void PopulateDisplayProperties(MainFormViewModel viewModel);
        void PopulateDisplayPropertiesAdmin(MainFormAdminViewModel viewModel);
        Task<MainFormAdminViewModel> PopulateDropdownsAndListsAdmin(MainFormAdminViewModel viewModel);
        Task<MainFormViewModel> PopulateDropdownsAndListsStaff(MainFormViewModel viewModel);
        void ValidateModel(IValidatableObject model, ModelStateDictionary modelState);
    }
}