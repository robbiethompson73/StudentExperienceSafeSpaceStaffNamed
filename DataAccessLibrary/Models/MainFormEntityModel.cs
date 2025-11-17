using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataAccessLibrary.Models
{
    public class MainFormEntityModel
    {
        public int Id { get; set; }
        public string SubmittedByWindowsUserName { get; set; }



        // Textboxes
        public string StaffFullName { get; set; }
        public string StaffTelephoneNumber { get; set; }
        public string StaffEmail { get; set; }
        public string IncidentPersonName { get; set; }
        public DateTime? IncidentDate { get; set; }


        // Textareas
        public string IncidentDetails { get; set; }



        // Radios
        public int IncidentHappenedToId { get; set; }
        public int NumberOfPeopleImpactedId { get; set; }
        public int NumberOfPeopleCausedIncidentId { get; set; }
        public int IncidentLocationId { get; set; }
        public int HasSimilarIncidentHappenedBeforeId { get; set; }






        // Dropdown Lists
        public int? StatusId { get; set; }

  
        // Checkboxes
        public List<int> SelectedImpactedPersonTypeIds { get; set; } = new(); // Instantiate to defend against NullReferenceExceptions
        public List<int> SelectedIncidentBehaviourTypeIds { get; set; } = new(); // Instantiate to defend against NullReferenceExceptions
        public List<int> SelectedIncidentMotivationTypeIds { get; set; } = new(); // Instantiate to defend against NullReferenceExceptions



        // Predefined
        public DateTime DateSubmitted { get; set; }


    }
}
