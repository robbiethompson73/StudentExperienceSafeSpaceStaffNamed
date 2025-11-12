using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServicesLibrary.FormPreparationModels
{
    public class ViewModelMappingContext
    {
        // Radios
        public List<SelectListItem> SampleRadioOptions { get; set; }
        public int? SelectedSampleRadioId { get; set; }
        public string? SelectedSampleRadioName { get; set; }


        // DropDownLists
        public List<SelectListItem> SampleDropdownOptions { get; set; }



        // Checkboxes
        public List<int> SelectedSampleCheckboxIds { get; set; }
        public List<SelectListItem> SampleCheckboxOptions { get; set; }
        public List<string> SelectedSampleCheckboxNames { get; set; }



        // Predefined
        public List<SelectListItem> StatusOptions { get; set; }
    }
}
