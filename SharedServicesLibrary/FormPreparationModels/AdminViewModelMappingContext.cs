using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServicesLibrary.FormPreparationModels
{
    public class AdminViewModelMappingContext : ViewModelMappingContext
    {

        // Radios



        // DropDownLists



        // Checkboxes




        /// <summary>
        /// Populated by AdminViewModelContextBuilder.BuildAsync()
        /// with the HTML‑formatted audit trail (most recent first).
        /// </summary>
        public string? AuditFormattedHtml { get; set; }

    }
}
