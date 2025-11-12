using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
    public class AdminStaffEntityModel
    {
        public int Id { get; set; }
        public string WindowsName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FormattedName { get; set; }
        public string ContactEmail { get; set; }
        public int DisplayOrder { get; set; }
        public bool ReceiveEmail { get; set; }
        public bool Active { get; set; }
    }
}
