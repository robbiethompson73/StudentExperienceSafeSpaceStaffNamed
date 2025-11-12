using SharedViewModelLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServicesLibrary.FormIconServices
{
    public class FormIconService : IFormIconService
    {
        public FormIconService()
        {

        }

        private readonly Dictionary<string, string> _iconMap = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Curriculum Discretionary Form", "fa-magic bg-purple text-bright-purple border-purple" },
            { "Education Support Assessment Arrangement Form", "fa-share-nodes bg-green text-bright-green border-green" },
            { "Student Misconduct Report Form", "fa-shield-halved bg-lightblue text-lightblue border-lightblue" },
            { "Counselling Referral Form", "fa-laptop-code bg-pink text-pink border-pink" },
            { "Report and Support Form - Anonymous", "fa-headset bg-orange text-orange border-orange" },
            { "Report and Support Form - Named", "fa-database bg-yellow text-yellow border-yellow" }
        };

        public void AssignIconClasses(StudentExperienceFormViewModel form)
        {
            var iconClassRaw = _iconMap.TryGetValue(form.Title, out var icon)
                ? icon
                : "fa-file bg-light text-secondary";

            var iconParts = iconClassRaw.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            form.IconClass = iconParts.FirstOrDefault(c => c.StartsWith("fa-")) ?? "fa-file";
            form.BackgroundClass = iconParts.FirstOrDefault(c => c.StartsWith("bg-")) ?? "bg-light";
            form.TextClass = iconParts.FirstOrDefault(c => c.StartsWith("text-")) ?? "text-secondary";
            form.BorderClass = iconParts.FirstOrDefault(c => c.StartsWith("border-")) ?? "border-secondary";

        }

        public void AssignIconClasses(IEnumerable<StudentExperienceFormViewModel> forms)
        {
            foreach (var form in forms)
            {
                AssignIconClasses(form);
            }
        }



    }
}
