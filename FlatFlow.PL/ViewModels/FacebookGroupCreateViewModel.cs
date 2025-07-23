using System.ComponentModel.DataAnnotations;

namespace FlatFlow.PL.ViewModels
{
    public class FacebookGroupCreateViewModel
    {
        [Required(ErrorMessage = "Group name is required")]
        [StringLength(200, ErrorMessage = "Group name cannot exceed 200 characters")]
        public string GroupName { get; set; }

        [Required(ErrorMessage = "Group link is required")]
        [Url(ErrorMessage = "Please enter a valid Facebook group URL")]
        public string GroupLink { get; set; }
    }
}
