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
    public class ExcelEntityModel
    {
        public int Id { get; set; }
        public DateTime DateSubmitted { get; set; }
        public string StudentReferenceNumber { get; set; }
        public DateTime? StudentDateOfBirth { get; set; }
        public string SubmittedByWindowsUserName { get; set; }
        public string StudentFullName { get; set; }
        public string SampleDropdownTitle { get; set; }

        public string SampleCheckboxTitles { get; set; }

    }
}
