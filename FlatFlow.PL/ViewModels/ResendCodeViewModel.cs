using System.ComponentModel.DataAnnotations;

namespace FlatFlow.PL.ViewModels
{
    public class ResendCodeViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

}
