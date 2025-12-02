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
        public DateTime? IncidentDate { get; set; }
        public string StaffFullName { get; set; }
        public string IncidentDetails { get; set; }
        public DateTime DateSubmitted { get; set; }

    }
}
