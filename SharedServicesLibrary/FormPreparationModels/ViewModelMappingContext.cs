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
        public List<SelectListItem> IncidentHappenedToOptions { get; set; }
        public int? SelectedIncidentHappenedToId { get; set; }
        public string? SelectedIncidentHappenedToName { get; set; }


        public List<SelectListItem> NumberOfPeopleImpactedOptions { get; set; }
        public int? SelectedNumberOfPeopleImpactedId { get; set; }
        public string? SelectedNumberOfPeopleImpactedName { get; set; }


        public List<SelectListItem> NumberOfPeopleCausedIncidentOptions { get; set; }
        public int? SelectedNumberOfPeopleCausedIncidentId { get; set; }
        public string? SelectedNumberOfPeopleCausedIncidentName { get; set; }


        public List<SelectListItem> IncidentLocationOptions { get; set; }
        public int? SelectedIncidentLocationId { get; set; }
        public string? SelectedIncidentLocationName { get; set; }


        public List<SelectListItem> HasSimilarIncidentHappenedBeforeOptions { get; set; }
        public int? SelectedHasSimilarIncidentHappenedBeforeId { get; set; }
        public string? SelectedHasSimilarIncidentHappenedBeforeName { get; set; }


        public List<SelectListItem> SampleRadioOptions { get; set; }
        public int? SelectedSampleRadioId { get; set; }
        public string? SelectedSampleRadioName { get; set; }





        // DropDownLists
        public List<SelectListItem> SampleDropdownOptions { get; set; }




        // Checkboxes
        public List<int> SelectedImpactedPersonTypeIds { get; set; }
        public List<SelectListItem> ImpactedPersonTypeOptions { get; set; }
        public List<string> SelectedImpactedPersonTypeNames { get; set; }

        public List<int> SelectedIncidentBehaviourTypeIds { get; set; }
        public List<SelectListItem> IncidentBehaviourTypeOptions { get; set; }
        public List<string> SelectedIncidentBehaviourTypeNames { get; set; }


        public List<int> SelectedIncidentMotivationTypeIds { get; set; }
        public List<SelectListItem> IncidentMotivationTypeOptions { get; set; }
        public List<string> SelectedIncidentMotivationTypeNames { get; set; }


        public List<int> SelectedSampleCheckboxIds { get; set; }
        public List<SelectListItem> SampleCheckboxOptions { get; set; }
        public List<string> SelectedSampleCheckboxNames { get; set; }










        // Predefined
        public List<SelectListItem> StatusOptions { get; set; }
    }
}
