using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServicesLibrary.AppSettings
{
    /// <summary>
    /// Represents global application settings defined in appsettings.json under "StatusSettings".
    /// </summary>
    public class StatusSettings
    {
        /// <summary>
        /// The title of the status to be used when a submitted status is required (e.g., "Open").
        /// This value is configured in appsettings.json under "StatusSettings:SubmittedTitle".
        /// </summary>
        public string SubmittedTitle { get; set; }
    }
}
