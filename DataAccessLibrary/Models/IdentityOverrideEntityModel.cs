using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
    public class IdentityOverrideEntityModel
    {
        public int? Id { get; set; }
        public string RealWindowsUsername { get; set; } = string.Empty;
        public string RealFormattedName { get; set; } = string.Empty;
        public string EffectiveWindowsUsername { get; set; } = string.Empty;
        public string EffectiveFormattedName { get; set; } = string.Empty;
    }
}
