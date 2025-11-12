using DataAccessLibrary.DataServices;
using DataAccessLibrary.Models;
using SharedViewModelLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AyrshireCollege.Biis.InputProcessingLibrary.InputProcessing.InputCleaner;

namespace SharedServicesLibrary.AdminStaffServices
{
    public class AdminStaffManagementService : IAdminStaffManagementService
    {
        private readonly IAdminStaffService _adminStaffServce;
        private readonly IAdminStaffMapper _adminStaffMapper;
        private readonly IWhitespaceTrimmer _whitespaceTrimmer;

        public AdminStaffManagementService(IAdminStaffService adminStaffServce,
                                            IAdminStaffMapper adminStaffMapper,
                                            IWhitespaceTrimmer whitespaceTrimmer)
        {
            _adminStaffServce = adminStaffServce;
            _adminStaffMapper = adminStaffMapper;
            _whitespaceTrimmer = whitespaceTrimmer;
        }


        public async Task<List<AdminStaffViewModel>> GetAdminStaffAllAsync()
        {
            // Retrieve all AdminStaff entities
            var adminStaffEntityList = await _adminStaffServce.GetAdminStaffAll();

            // Prepare a list to hold the converted ViewModel objects
            var viewModelList = new List<AdminStaffViewModel>();

            // Loop through each entity model retrieved
            foreach (var entity in adminStaffEntityList)
            {
                // Convert the entity model to a ViewModel using the mapper
                var adminStaffViewModel = _adminStaffMapper.ToViewModel(entity);

                // Add the converted ViewModel to the list
                viewModelList.Add(adminStaffViewModel);
            }

            // Return the list of AdminStaffViewModel objects
            return viewModelList;
        }


        public async Task<AdminStaffViewModel> GetAdminStaffByIdAsync(int adminStaffId)
        {
            // Retrieve Entity
            var adminStaffEntity = await _adminStaffServce.GetAdminStaffById(adminStaffId);

            // Convert Entity to ViewModel
            var adminStaffViewModel = _adminStaffMapper.ToViewModel(adminStaffEntity);

            // Return the ViewModel
            return adminStaffViewModel;
        }






        public async Task<int> CreateAdminStaffAsync(AdminStaffViewModel adminStaffViewModel)
        {
            var windowsName = _whitespaceTrimmer.TrimAllWhitespace(adminStaffViewModel.WindowsName);
            windowsName = _whitespaceTrimmer.ReplaceSpaces(windowsName, "");
            windowsName = windowsName.ToLower();
            adminStaffViewModel.WindowsName = windowsName;


            var firstName = _whitespaceTrimmer.TrimAllWhitespace(adminStaffViewModel.FirstName);
            adminStaffViewModel.FirstName = firstName;

            var lastName = _whitespaceTrimmer.TrimAllWhitespace(adminStaffViewModel.LastName);
            lastName = _whitespaceTrimmer.ReplaceSpaces(lastName, "");
            adminStaffViewModel.LastName = lastName;

            var contactEmail = _whitespaceTrimmer.TrimAllWhitespace(adminStaffViewModel.ContactEmail);
            contactEmail = _whitespaceTrimmer.ReplaceSpaces(contactEmail, "");
            contactEmail = contactEmail.ToLower();
            adminStaffViewModel.ContactEmail = contactEmail;

            // Map ViewModel to Entity
            var adminStaffEntityModel = _adminStaffMapper.ToEntity(adminStaffViewModel);

            // Perform Insert
            int submissionId = await _adminStaffServce.CreateAsync(adminStaffEntityModel);

            return submissionId;
        }




        public async Task<int> UpdateAdminStaffAsync(AdminStaffViewModel adminStaffViewModel)
        {

            // STILL TO DO
            // Add class to InputProcessingLibraryApp and push NuGet
            // Move methods Trim() and ReplaceSpaces() to class

            var windowsName = _whitespaceTrimmer.TrimAllWhitespace(adminStaffViewModel.WindowsName);
            windowsName = _whitespaceTrimmer.ReplaceSpaces(windowsName, "");
            windowsName = windowsName.ToLower();
            adminStaffViewModel.WindowsName = windowsName;

            var firstName = _whitespaceTrimmer.TrimAllWhitespace(adminStaffViewModel.FirstName);
            firstName = _whitespaceTrimmer.ReplaceSpaces(firstName, "");
            adminStaffViewModel.FirstName = firstName;

            var lastName = _whitespaceTrimmer.TrimAllWhitespace(adminStaffViewModel.LastName);
            lastName = _whitespaceTrimmer.ReplaceSpaces(lastName, "");
            adminStaffViewModel.LastName = lastName;

            var contactEmail = _whitespaceTrimmer.TrimAllWhitespace(adminStaffViewModel.ContactEmail);
            contactEmail = _whitespaceTrimmer.ReplaceSpaces(contactEmail, "");
            contactEmail = contactEmail.ToLower();
            adminStaffViewModel.ContactEmail = contactEmail;

            var adminStaffEntityModel = _adminStaffMapper.ToEntity(adminStaffViewModel);

            int submissionId = await _adminStaffServce.UpdateAsync(adminStaffEntityModel);

            return submissionId;
        }







    }
}
