using DataAccessLibrary.Models;
using SharedViewModelLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServicesLibrary.StudentExperienceFormServices
{
    public class StudentExperienceFormMapper : IStudentExperienceFormMapper
    {
        public StudentExperienceFormMapper()
        {
        }

        public StudentExperienceFormEntityModel ToEntity(StudentExperienceFormViewModel viewModel)
        {
            return new StudentExperienceFormEntityModel
            {
                Id = viewModel.Id,
                Title = (viewModel.Title),
                Description = viewModel.Description,
                Url = viewModel.Url,
                Active = viewModel.Active,
                Order = viewModel.Order
            };
        }

        public StudentExperienceFormViewModel ToViewModel(StudentExperienceFormEntityModel entity)
        {
            return new StudentExperienceFormViewModel
            {
                Id = entity.Id,
                Title = (entity.Title),
                Description = entity.Description,
                Url = entity.Url,
                Active = entity.Active,
                Order = entity.Order
            };
        }



    }
}
