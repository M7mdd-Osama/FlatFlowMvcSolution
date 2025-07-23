using System.ComponentModel.DataAnnotations;

namespace FlatFlow.PL.ViewModels
{
    public class FacebookGroupEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Group name is required")]
        [StringLength(200, ErrorMessage = "Group name cannot exceed 200 characters")]
        [Display(Name = "Group Name")]
        public string GroupName { get; set; }

        [Required(ErrorMessage = "Group link is required")]
        [Url(ErrorMessage = "Please enter a valid Facebook group URL")]
        [Display(Name = "Facebook Group Link")]
        public string GroupLink { get; set; }
    }
}
