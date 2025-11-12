using DataAccessLibrary.DataServices;
using DataAccessLibrary.Models;
using SharedViewModelLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AyrshireCollege.Biis.InputProcessingLibrary.InputProcessing.InputCleaner;

namespace SharedServicesLibrary.StudentExperienceFormServices
{
    public class StudentExperienceFormManagementService : IStudentExperienceFormManagementService
    {
        private readonly IStudentExperienceFormService _studentExperienceFormService;
        private readonly IStudentExperienceFormMapper _studentExperienceFormMapper;

        public StudentExperienceFormManagementService(IStudentExperienceFormService studentExperienceFormService,
                                                    IStudentExperienceFormMapper studentExperienceFormMapper)
        {
            _studentExperienceFormService = studentExperienceFormService;
            _studentExperienceFormMapper = studentExperienceFormMapper;
        }



        public async Task<List<StudentExperienceFormViewModel>> GetStudentExperienceFormActiveAsync()
        {
            // Retrieve all form entities
            var entityList = await _studentExperienceFormService.GetStudentExperienceFormActive();

            // Prepare a list to hold the converted ViewModel objects
            var viewModelList = new List<StudentExperienceFormViewModel>();

            // Loop through each entity model retrieved
            foreach (var entity in entityList)
            {
                // Convert the entity model to a ViewModel using the mapper
                var studentExperienceFormViewModel = _studentExperienceFormMapper.ToViewModel(entity);

                // Add the converted ViewModel to the list
                viewModelList.Add(studentExperienceFormViewModel);
            }

            // Return the list of AdminStaffViewModel objects
            return viewModelList;
        }









    }
}
