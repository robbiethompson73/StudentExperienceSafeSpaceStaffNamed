using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailLibrary.Configuration
{
    public class EmailSettings
    {
        public string FromEmail { get; set; } = string.Empty;
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string? Username { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;

        // Populated from appsettings.json via IOptions<EmailSettings> in DI.
        public bool OverrideAdminRecipients { get; set; }

        // The override email address to use for admin notifications if overriding is enabled.
        // Populated from appsettings.json via DI.
        public string AdminEmailOverride { get; set; }

        // Indicates whether user email recipients should be overridden (e.g., for testing).
        // Populated from appsettings.json via DI.
        public bool OverrideUserRecipients { get; set; }

        // The override email address to use for user notifications if overriding is enabled.
        // Populated from appsettings.json via DI.
        public string UserEmailOverride { get; set; }

        // Specifies environments (e.g., "Development", "UAT") where BIIS Admin should be CC’d.
        // Populated from appsettings.json via DI.
        public List<string> SendBiisAdminCcEnvironments { get; set; } = new();
    }


}
