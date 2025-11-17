using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailLibrary.Models
{
    public class FormSubmissionEmailNotificationUserModel
    {
        public FormSubmissionEmailNotificationUserModel()
        {
        }

        public string FormName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string TemplatePath { get; set; } = string.Empty;
        public string TargetUserEmail { get; set; } = string.Empty;
        public string OriginalRecipientUserEmail { get; set; } = string.Empty;


        // Properties For Display In User Email
        public int Id { get; set; }
        public string SubmittedBy { get; set; } = string.Empty;

        public string SubmittedByFirstNamesOnly =>
                        string.Join(" ", SubmittedBy.Split(' ', StringSplitOptions.RemoveEmptyEntries).Reverse().Skip(1).Reverse());

        public string StaffFullName { get; set; } = string.Empty;

        public DateTime? IncidentDate { get; set; }

        public string IncidentLocationName { get; set; } = string.Empty;

        public DateTime SubmittedAt { get; set; } = DateTime.Now; // or DateTime.UtcNow

    }
}
