using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
    public class MainFormAuditLogEntityModel
    {
        public int MainFormId { get; set; }
        public string PropertyName { get; set; }
        public string DisplayName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string ChangedBy { get; set; }
        public DateTime ChangeDate { get; set; } = DateTime.UtcNow;
    }
}
