using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServicesLibrary.AppSettings
{
    /// <summary>
    /// Represents global application settings defined in appsettings.json under "GlobalSettings".
    /// </summary>
    public class GlobalSettings
    {
        /// <summary>
        /// A display name or title used throughout the solution.
        /// Typically configured in appsettings.json under "GlobalSettings:MyGlobalName".
        /// </summary>
        public string MyGlobalName { get; set; }
    }
}
