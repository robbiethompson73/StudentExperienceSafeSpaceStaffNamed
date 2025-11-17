using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailLibrary.Models
{
    public class FormSubmissionEmailNotificationAdminModel
    {
        private readonly string _baseUrl;
        public FormSubmissionEmailNotificationAdminModel(string baseUrl)
        {
            _baseUrl = baseUrl;
        }
        public string FormName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string TemplatePath { get; set; } = string.Empty;

        public List<string> TargetAdminEmails { get; set; } = new List<string>();  // List of Admin emails
        public string OriginalRecipientAdminEmail { get; set; } = string.Empty;


        // Properties For Display In User Email
        public int Id { get; set; }
        public string SubmittedBy { get; set; } = string.Empty;

        public string StaffFullName { get; set; } = string.Empty;
        public DateTime? IncidentDate { get; set; }
        public string IncidentLocationName { get; set; } = string.Empty;


        // Read-only property that builds the link to the details page
        public string DetailsLink => $"{_baseUrl}/Admin/Details/{Id}";

        // Read-only property that builds the link to the view all records page
        public string AdminListLink => $"{_baseUrl}/Admin/List";


    }
}
