using AyrshireCollege.Biis.InputProcessingLibrary.InputProcessing.ValidationAttributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using SharedViewModelLibrary.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SharedViewModelLibrary.Models
{
    public class MainFormAdminViewModel : MainFormViewModel,IValidatableObject
    {
        public MainFormAdminViewModel()
        {
        }

        // Textboxes
        [Required(ErrorMessage = "Name of Admin is required.")]
        [NoWhitespaceOnly]
        [StringLength(100, ErrorMessage = "Admin name cannot exceed 100 characters.")]
        [Display(Name = "Name of Admin")]
        public string StaffMemberAssignedAdmin { get; set; }




        // Textareas
        [Display(Name = "Admin Notes")]
        public string? AdminNote { get; set; }

        [Required(ErrorMessage = "Details of action taken is required.")]
        [StringLength(40000, ErrorMessage = "Action details cannot exceed 40000 characters.")]
        [Display(Name = "Details of action taken")]
        public string ActionTakenByCollegeAdmin { get; set; }




        // Radio - SampleRadioAdmin


        // Dropdown Lists




        // Checkboxes







        // Override the base class property to make 'Status' required in the Admin context.
        // 'new' hides the base class property from MainFormViewModel.
        // '[Required]' enforces that a status must be selected (non-zero value).
        // '[Display]' provides the label text used in validation messages and UI helpers.
        [Required(ErrorMessage = "Status is required.")]
        [Display(Name = "Status")]
        public int? AdminStatusId { get; set; } // Nullable int allows Required to detect "unselected" state



        [Display(Name = "Admin Audit")]
        public string? AdminAuditFormatted { get; set; }








        // Override validation logic for admin-specific scenarios
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Reuse checkbox group validation from base class
            foreach (var result in ValidateCheckboxGroups())
                yield return result;

            // Add admin-specific validation here if needed (e.g., required status or comments)
            if (StatusId == 0)
            {
                yield return new ValidationResult("Status must be selected. SERVER SIDE", new[] { nameof(StatusId) });
            }

        }






    }
}
