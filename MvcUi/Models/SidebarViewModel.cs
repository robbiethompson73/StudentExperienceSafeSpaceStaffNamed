using SharedViewModelLibrary.Models;

namespace MvcUi.Models
{
    public class SidebarViewModel
    {
        public string UserRole { get; set; }

        public string FormName { get; set; }

        public IEnumerable<StudentExperienceFormViewModel> Forms { get; set; }
    }
}
