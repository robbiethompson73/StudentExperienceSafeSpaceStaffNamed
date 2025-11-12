using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServicesLibrary.FormPreparationModels
{
    public class AdminViewModelMappingContext : ViewModelMappingContext
    {

        // Radios
        public List<SelectListItem> SampleRadioAdminOptions { get; set; }
        public int? SelectedSampleRadioAdminId { get; set; }
        public string? SelectedSampleRadioAdminName { get; set; }



        // DropDownLists
        public List<SelectListItem> SampleDropdownAdminOptions { get; set; }



        // Checkboxes
        public List<int> SelectedSampleCheckboxAdminIds { get; set; }
        public List<SelectListItem> SampleCheckboxAdminOptions { get; set; }
        public List<string> SelectedSampleCheckboxAdminNames { get; set; }





        /// <summary>
        /// Populated by AdminViewModelContextBuilder.BuildAsync()
        /// with the HTML‑formatted audit trail (most recent first).
        /// </summary>
        public string? AuditFormattedHtml { get; set; }

    }
}
