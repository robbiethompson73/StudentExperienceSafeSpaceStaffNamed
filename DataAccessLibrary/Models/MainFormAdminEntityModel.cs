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
    public class MainFormAdminEntityModel : MainFormEntityModel
    {

        // Textboxes
        
        public string StaffMemberAssignedAdmin { get; set; }
        public string SampleTextboxAdmin { get; set; }
        public DateTime? SampleDateAdmin { get; set; } // Date
        public decimal? SampleCostAdmin { get; set; }


        // Textareas
        public string ActionTakenByCollegeAdmin { get; set; }
        public string AdminNote { get; set; }
        public string SampleTextareaAdmin { get; set; }


        // Radios
        public int SampleRadioAdminId { get; set; }


        // Dropdown Lists
        public int? SampleDropdownAdminId { get; set; }


        // Checkboxes
        public List<int> SelectedSampleCheckboxAdminIds { get; set; } = new(); // Instantiate to defend against NullReferenceExceptions




    }
}
