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


        // Textareas
        public string ActionTakenByCollegeAdmin { get; set; }
        public string AdminNote { get; set; }


        // Radios


        // Dropdown Lists


        // Checkboxes




    }
}
