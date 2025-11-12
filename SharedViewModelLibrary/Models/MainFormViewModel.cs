using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AyrshireCollege.Biis.InputProcessingLibrary.InputProcessing.ValidationAttributes;
using SharedViewModelLibrary.Validation;

namespace SharedViewModelLibrary.Models
{
    public class MainFormViewModel : IValidatableObject
    {
        public MainFormViewModel()
        {
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Student Reference Number is required.")]
        [NoWhitespaceOnly]
        [StringLength(50, ErrorMessage = "Student Reference Number cannot exceed 50 characters.")]
        [RegularExpression(@"^[Aa]\d{7}$", ErrorMessage = "Student Reference Number must start with 'A' (uppercase or lowercase) followed by 7 digits (e.g., A1234567 or a1234567).")]
        [Display(Name = "Student Reference Number")]
        public string StudentReferenceNumber { get; set; }


        [Required(ErrorMessage = "Please select a Student Date of Birth.")]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? StudentDateOfBirth { get; set; }


        [BindNever] // not included in ModalState.IsValid. BindNever reference types Must be nullable
        public string? SubmittedByWindowsUserName { get; set; }

        [BindNever] // not included in ModalState.IsValid. BindNever reference types Must be nullable
        public string? SubmittedByDisplayName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SubmittedByWindowsUserName))
                    return null;

                var parts = SubmittedByWindowsUserName.Split('.');
                return string.Join(" ", parts.Select(p =>
                    char.ToUpper(p[0]) + p.Substring(1).ToLower()));
            }
        }



        [Required(ErrorMessage = "Full Name is required.")]
        [NoWhitespaceOnly]
        [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
        [Display(Name = "Student Full Name")]
        public string StudentFullName { get; set; }




        [Required(ErrorMessage = "SampleTextbox is required.")]
        [StringLength(50, ErrorMessage = "SampleTextbox cannot exceed 50 characters.")]
        [Display(Name = "SampleTextbox")]
        public string SampleTextbox { get; set; }


        // Sample Date picker field
        [Required(ErrorMessage = "Please select a Date.")]
        [Display(Name = "Sample Date")]
        [DataType(DataType.Date)]
        public DateTime? SampleDate { get; set; }


        // Time picker field
        [Required(ErrorMessage = "Please enter a Time.")]
        [Display(Name = "SampleTime")]
        [DataType(DataType.Time)]
        public TimeSpan? SampleTime { get; set; }


        // Sample Cost
        [Required(ErrorMessage = "Please enter a Cost.")]
        [Range(0.01, 9999999.99, ErrorMessage = "Please enter a valid amount.")]
        [Display(Name = "Sample Cost")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SampleCost { get; set; } = 0.00m; // Ensure it initializes correctly


        // Textarea
        [Required(ErrorMessage = "SampleTextarea is required.")]
        [StringLength(20000, ErrorMessage = "SampleTextarea cannot exceed 20000 characters.")]
        [Display(Name = "Sample Textarea")]
        public string SampleTextarea { get; set; }



        // Radio - SampleRadio
        [Required(ErrorMessage = "Please select an option.")]
        [Display(Name = "Sample Radio")]
        public int? SelectedSampleRadioId { get; set; } // Bound to radio buttons
        public string? SampleRadioName { get; set; }
        public List<SelectListItem> SampleRadioOptions { get; set; } = new();



        // Dropdown Lists
        [Required(ErrorMessage = "SampleDropdown is required.")]
        [Display(Name = "SampleDropdown")]
        public int? SampleDropdownId { get; set; }

        [ValidateNever]
        public List<SelectListItem> SampleDropdownOptions { get; set; }

        // Custom Convenience Property
        public string SampleDropdownDisplayName { get; set; } = string.Empty;





        // Checkbox Group read from tblSampleCheckbox
        [Display(Name = "Selected SampleCheckbox")]
        public List<int> SelectedSampleCheckboxIds { get; set; } = new(); // Instantiate to defend against NullReferenceExceptions

        // Holds the list of checkbox <options> (Id and Name) that render as checkboxes in the view
        public List<SelectListItem> SampleCheckboxOptions { get; set; } = new();

        // Store the Names (not IDs) of the options selected by the user
        public List<string> SelectedSampleCheckboxNames { get; set; } = new(); // To display on the view

        // Server-Side validation helper to check whether the user selected at least one checkbox from a group
        public bool HasSelectedSampleCheckbox { get; set; }





        public string? Mode { get; set; } = "Create"; // "Create" or "Update" set in Controller based on which action is being called



        [BindNever] // not included in ModalState.IsValid. BindNever reference types Must be nullable
        [Display(Name = "Submission date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public DateTime DateSubmitted { get; set; }






        // Status DDL
        [Required(ErrorMessage = "Status is required.")]
        [Display(Name = "Status")]
        public int? StatusId { get; set; }

        // List of SelectListItems for Status
        [ValidateNever]
        public List<SelectListItem> StatusOptions { get; set; }

        // Custom Convenience Property
        public string StatusDisplayName { get; set; } = string.Empty;







        // Base validation method called during model validation
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validate that at least one checkbox is selected in each group (samplecheckbox)
            foreach (var result in ValidateCheckboxGroups())
                yield return result;
        }

        // Shared checkbox group validation for reuse in derived classes
        protected IEnumerable<ValidationResult> ValidateCheckboxGroups()
        {

            // Validate that at least one SampleCheckbox is selected
            if (SelectedSampleCheckboxIds == null || SelectedSampleCheckboxIds.Count == 0)
            {
                yield return new ValidationResult("Please select at least one SampleCheckbox option.", new[] { nameof(HasSelectedSampleCheckbox) });
            }


        }






    }
}
