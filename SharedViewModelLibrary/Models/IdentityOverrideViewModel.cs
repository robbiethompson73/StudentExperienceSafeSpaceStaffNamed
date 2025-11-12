using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedViewModelLibrary.Models
{
    public class IdentityOverrideViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Please enter the real (target) user's Windows username.")]
        [StringLength(50, ErrorMessage = "The Windows username must be 50 characters or fewer.")]
        [RegularExpression(@"^[a-zA-Z0-9]+\.[a-zA-Z0-9]+$",
            ErrorMessage = "The Windows username must be in the format 'username.domain' using only letters and numbers.")]
        [Display(Name = "Real Windows Username")]
        public string RealWindowsUsername { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the real (target) user's display name.")]
        [StringLength(50, ErrorMessage = "The display name must be 50 characters or fewer.")]
        [Display(Name = "Real Display Name")]
        public string RealFormattedName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the effective (override) user's Windows username.")]
        [StringLength(50, ErrorMessage = "The Windows username must be 50 characters or fewer.")]
        [RegularExpression(@"^[a-zA-Z0-9]+\.[a-zA-Z0-9]+$",
            ErrorMessage = "The Windows username must be in the format 'username.domain' using only letters and numbers.")]
        [Display(Name = "Effective (Override) Windows Username")]
        public string EffectiveWindowsUsername { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the effective (override) user's display name.")]
        [StringLength(50, ErrorMessage = "The display name must be 50 characters or fewer.")]
        [Display(Name = "Effective (Override) Display Name")]
        public string EffectiveFormattedName { get; set; } = string.Empty;


    }
}
