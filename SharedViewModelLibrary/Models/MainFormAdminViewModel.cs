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


        [Required(ErrorMessage = "SampleTextboxAdmin is required.")]
        [NoWhitespaceOnly]
        [StringLength(50, ErrorMessage = "SampleTextboxAdmin cannot exceed 50 characters.")]
        [Display(Name = "Sample Textbox Admin")]
        public string SampleTextboxAdmin { get; set; }


        [Required(ErrorMessage = "Please select a date.")]
        [Display(Name = "Sample Date Admin")]
        [DataType(DataType.Date)]
        public DateTime? SampleDateAdmin { get; set; }


        // Backing property
        private decimal? _sampleCostAdmin;

        // SampleCostAdmin
        [Required(ErrorMessage = "Please enter a Sample Admin Cost.")]

        // Allows values from 0.00 up to 9,999,999.99 (including 0.00). 
        // Useful when zero is considered a valid entry (e.g., free or no cost).
        // [Range(0, 9999999.99, ErrorMessage = "Please enter a valid amount.")]

        // Allows values from 0.01 up to 9,999,999.99, excluding 0.00.
        // Useful when zero is not valid and a positive, non-zero amount is required.
        [Range(0.01, 9999999.99, ErrorMessage = "Please enter a valid amount.")]
        [Display(Name = "Sample Cost Admin")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SampleCostAdmin
        {
            get => _sampleCostAdmin ?? 0.00m; // Ensure it initializes correctly
            set => _sampleCostAdmin = value;
        }



        // Textareas
        [Display(Name = "Admin Notes")]
        public string? AdminNote { get; set; }

        [Required(ErrorMessage = "Details of action taken is required.")]
        [StringLength(40000, ErrorMessage = "Action details cannot exceed 40000 characters.")]
        [Display(Name = "Details of action taken")]
        public string ActionTakenByCollegeAdmin { get; set; }


        [Required(ErrorMessage = "SampleTextareaAdmin is required.")]
        [StringLength(20000, ErrorMessage = "SampleTextareaAdmin cannot exceed 20000 characters.")]
        [Display(Name = "Sample Textarea Admin")]
        public string SampleTextareaAdmin { get; set; }



        // Radio - SampleRadioAdmin
        [Required(ErrorMessage = "Please select an option.")]
        [Display(Name = "Sample Radio Admin")]
        public int? SelectedSampleRadioAdminId { get; set; } // Bound to radio buttons
        public string? SampleRadioAdminName { get; set; }
        public List<SelectListItem> SampleRadioAdminOptions { get; set; } = new();


        // Dropdown Lists
        [Required(ErrorMessage = "SampleDropdownAdmin is required.")]
        [Display(Name = "Sample Dropdown Admin")]
        public int? SampleDropdownAdminId { get; set; }

        [ValidateNever]
        public List<SelectListItem> SampleDropdownAdminOptions { get; set; }

        // Custom Convenience Property
        public string SampleDropdownAdminDisplayName { get; set; } = string.Empty;





        // Checkboxes
        //
        // Sample Checkbox Group read from tblSampleCheckboxAdmin
        [Display(Name = "Sample Checkbox Admin")]
        public List<int> SelectedSampleCheckboxAdminIds { get; set; } = new(); // Instantiate to defend against NullReferenceExceptions

        // Holds the list of checkbox <options> (Id and Name) that render as checkboxes in the view
        public List<SelectListItem> SampleCheckboxAdminOptions { get; set; } = new();

        // Store the Names (not IDs) of the options selected by the user
        public List<string> SelectedSampleCheckboxAdminNames { get; set; } = new(); // To display on the view

        // Server-Side validation helper to check whether the user selected at least one checkbox from a group
        public bool HasSelectedSampleCheckboxAdmin { get; set; }







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
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
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
