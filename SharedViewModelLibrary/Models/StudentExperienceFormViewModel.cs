using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AyrshireCollege.Biis.InputProcessingLibrary.InputProcessing.ValidationAttributes;


namespace SharedViewModelLibrary.Models
{
    public class StudentExperienceFormViewModel
    {
        public StudentExperienceFormViewModel()
        {
        }

        public int Id { get; set; }


        [Required(ErrorMessage = "Title is required.")]
        [StringLength(255, ErrorMessage = "Title cannot exceed 255 characters.")]
        [Display(Name = "Title")]
        public string Title { get; set; }


        [Required(ErrorMessage = "Description are required.")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "URL is required.")]
        [StringLength(255, ErrorMessage = "URL cannot exceed 255 characters.")]
        [Display(Name = "URL")]
        public string Url { get; set; }

        // Radio Buttons for Active preference
        [Required(ErrorMessage = "An active state is required.")]
        [Display(Name = "Active State")]
        public bool Active { get; set; }

        [Required(ErrorMessage = "An Order value is required.")]
        [Display(Name = "Order")]
        public int Order { get; set; }


        public string IconClass { get; set; }
        public string BackgroundClass { get; set; }
        public string TextClass { get; set; }
        public string BorderClass { get; set; }

    }
}
