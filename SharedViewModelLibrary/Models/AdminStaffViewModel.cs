using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AyrshireCollege.Biis.InputProcessingLibrary.InputProcessing.ValidationAttributes;


namespace SharedViewModelLibrary.Models
{
    public class AdminStaffViewModel
    {
        public AdminStaffViewModel()
        {
        }

        public int Id { get; set; }


        [Required(ErrorMessage = "Windows Name is required.")]
        [StringLength(255, ErrorMessage = "Windows Name cannot exceed 255 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9]+\.[a-zA-Z0-9]+$", ErrorMessage = "Windows Name must be in the format 'name.domain' using only letters and numbers.")]
        [Display(Name = "Windows Name")]
        public string? WindowsName { get; set; }


        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(255, ErrorMessage = "First Name cannot exceed 255 characters.")]
        [Display(Name = "First Name(s)")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(255, ErrorMessage = "Last Name cannot exceed 255 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [BindNever] // // not included in ModalState.IsValid. BindNever reference types Must be nullable
        public string? FormattedName => $"{FirstName} {LastName}";


        [Required(ErrorMessage = "Email is required.")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [Display(Name = "Contact Email Address")]
        public string ContactEmail { get; set; }


        [Required(ErrorMessage = "An email notification selection is required.")]
        [Display(Name = "Receive Email Notifications")]
        public bool ReceiveEmail { get; set; }


        // Radio Buttons for Active preference
        [Required(ErrorMessage = "An active state is required.")]
        [Display(Name = "Active State")]
        public bool Active { get; set; }



    }
}
