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

        [Required(ErrorMessage = "Staff name is required.")]
        [NoWhitespaceOnly]
        [StringLength(100, ErrorMessage = "Staff name cannot exceed 100 characters.")]
        [Display(Name = "Staff Full Name")]
        public string StaffFullName { get; set; }

        [NoWhitespaceOnly]
        [StringLength(100, ErrorMessage = "Staff mobile or phone number cannot exceed 100 characters.")]
        [Display(Name = "Mobile or Phone Number")]
        public string? StaffTelephoneNumber { get; set; }


        [Required(ErrorMessage = "Staff Email address is required.")]
        [StringLength(100, ErrorMessage = "Email address cannot exceed 100 characters.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [RegularExpression(@"^[^@\s]+@ayrshire\.ac\.uk$", ErrorMessage = "Please enter a valid Ayrshire College email address (must end with @ayrshire.ac.uk).")]
        [Display(Name = "Staff Email")]
        public string StaffEmail { get; set; }


        [NoWhitespaceOnly]
        [StringLength(100, ErrorMessage = "Affected person name cannot exceed 100 characters.")]
        [Display(Name = "If you know who this happened to, can you give us their name?")]
        public string? IncidentPersonName { get; set; }


        [Required(ErrorMessage = "Please select a Student Date of Birth.")]
        [Display(Name = "When did the incident happen?")]
        [DataType(DataType.Date)]
        public DateTime? IncidentDate { get; set; }

        



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




        // Textarea
        [Required(ErrorMessage = "Incident Details is required.")]
        [StringLength(40000, ErrorMessage = "Incident Details cannot exceed 40000 characters.")]
        [Display(Name = "Please tell us as much as you can about this incident")]
        public string IncidentDetails { get; set; }




        // Radio - IncidentHappenedToOptions
        [Required(ErrorMessage = "Please select an option.")]
        [Display(Name = "Who did the incident happen to?")]
        public int? SelectedIncidentHappenedToId { get; set; } // Bound to radio buttons
        public string? IncidentHappenedToName { get; set; }
        public List<SelectListItem> IncidentHappenedToOptions { get; set; } = new();


        [Required(ErrorMessage = "Please select an option.")]
        [Display(Name = "How many people were impacted by this incident?")]
        public int? SelectedNumberOfPeopleImpactedId { get; set; } // Bound to radio buttons
        public string? NumberOfPeopleImpactedName { get; set; }
        public List<SelectListItem> NumberOfPeopleImpactedOptions { get; set; } = new();


        [Required(ErrorMessage = "Please select an option.")]
        [Display(Name = "How many people allegedly caused this incident?")]
        public int? SelectedNumberOfPeopleCausedIncidentId { get; set; } // Bound to radio buttons
        public string? NumberOfPeopleCausedIncidentName { get; set; }
        public List<SelectListItem> NumberOfPeopleCausedIncidentOptions { get; set; } = new();


        [Required(ErrorMessage = "Please select an option.")]
        [Display(Name = "Where did it happen?")]
        public int? SelectedIncidentLocationId { get; set; } // Bound to radio buttons
        public string? IncidentLocationName { get; set; }
        public List<SelectListItem> IncidentLocationOptions { get; set; } = new();


        [Required(ErrorMessage = "Please select an option.")]
        [Display(Name = "Has a similar incident happened before?")]
        public int? SelectedHasSimilarIncidentHappenedBeforeId { get; set; } // Bound to radio buttons
        public string? HasSimilarIncidentHappenedBeforeName { get; set; }
        public List<SelectListItem> HasSimilarIncidentHappenedBeforeOptions { get; set; } = new();







        // Dropdown Lists











        // Checkbox Group read from tblImpactedPersonType
        [Display(Name = "Do you know whether the person/persons impacted by this incident were?")]
        public List<int> SelectedImpactedPersonTypeIds { get; set; } = new(); // Instantiate to defend against NullReferenceExceptions

        // Holds the list of checkbox <options> (Id and Name) that render as checkboxes in the view
        public List<SelectListItem> ImpactedPersonTypeOptions { get; set; } = new();

        // Store the Names (not IDs) of the options selected by the user
        public List<string> SelectedImpactedPersonTypeNames { get; set; } = new(); // To display on the view

        // Server-Side validation helper to check whether the user selected at least one checkbox from a group
        public bool HasSelectedImpactedPersonType { get; set; }





        // Checkbox Group read from tblIncidentBehaviourType
        [Display(Name = "Did the incident involve any of the following?")]
        public List<int> SelectedIncidentBehaviourTypeIds { get; set; } = new(); // Instantiate to defend against NullReferenceExceptions

        // Holds the list of checkbox <options> (Id and Name) that render as checkboxes in the view
        public List<SelectListItem> IncidentBehaviourTypeOptions { get; set; } = new();

        // Store the Names (not IDs) of the options selected by the user
        public List<string> SelectedIncidentBehaviourTypeNames { get; set; } = new(); // To display on the view

        // Server-Side validation helper to check whether the user selected at least one checkbox from a group
        public bool HasSelectedIncidentBehaviourType { get; set; }



        // Checkbox Group read from tblIncidentMotivationType
        [Display(Name = "Do you believe the incident was motivated by any of the following?")]
        public List<int> SelectedIncidentMotivationTypeIds { get; set; } = new(); // Instantiate to defend against NullReferenceExceptions

        // Holds the list of checkbox <options> (Id and Name) that render as checkboxes in the view
        public List<SelectListItem> IncidentMotivationTypeOptions { get; set; } = new();

        // Store the Names (not IDs) of the options selected by the user
        public List<string> SelectedIncidentMotivationTypeNames { get; set; } = new(); // To display on the view

        // Server-Side validation helper to check whether the user selected at least one checkbox from a group
        public bool HasSelectedIncidentMotivationType { get; set; }





        public string? Mode { get; set; } = "Create"; // "Create" or "Update" set in Controller based on which action is being called



        [BindNever] // not included in ModalState.IsValid. BindNever reference types Must be nullable
        [Display(Name = "Submission Date")]
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
            // Only require IncidentPersonName if SelectedIncidentHappenedToId == 2
            //if (SelectedIncidentHappenedToId.HasValue && SelectedIncidentHappenedToId.Value == 2)
            //{
            //    if (string.IsNullOrWhiteSpace(IncidentPersonName))
            //    {
            //        yield return new ValidationResult(
            //            "Affected person name is required.",
            //            new[] { nameof(IncidentPersonName) }
            //        );
            //    }
            //}


            // Existing checkbox validation
            foreach (var result in ValidateCheckboxGroups())
                yield return result;
        }




        // Shared checkbox group validation for reuse in derived classes
        protected IEnumerable<ValidationResult> ValidateCheckboxGroups()
        {

            if (SelectedImpactedPersonTypeIds == null || SelectedImpactedPersonTypeIds.Count == 0)
            {
                yield return new ValidationResult("Please select at least one Impacted Person Type option.", new[] { nameof(HasSelectedImpactedPersonType) });
            }


            if (SelectedIncidentBehaviourTypeIds == null || SelectedIncidentBehaviourTypeIds.Count == 0)
            {
                yield return new ValidationResult("Please select at least one Incident Behaviour Type option.", new[] { nameof(HasSelectedIncidentBehaviourType) });
            }


            if (SelectedIncidentMotivationTypeIds == null || SelectedIncidentMotivationTypeIds.Count == 0)
            {
                yield return new ValidationResult("Please select at least one Incident Motivation Type option.", new[] { nameof(HasSelectedIncidentMotivationType) });
            }


        }






    }
}
