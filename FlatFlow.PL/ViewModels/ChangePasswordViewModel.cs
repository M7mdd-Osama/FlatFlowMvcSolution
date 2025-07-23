using System.ComponentModel.DataAnnotations;

namespace FlatFlow.PL.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = null!;

        [Required]
        [Display(Name = "Confirm New Password")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
