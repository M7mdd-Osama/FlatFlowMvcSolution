using System.ComponentModel.DataAnnotations;

namespace FlatFlow.PL.ViewModels
{
    public class EditApartmentViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public bool IsRented { get; set; }

        [Display(Name = "Add New Images")]
        public List<IFormFile>? Images { get; set; }

        [Display(Name = "Current Images")]
        public List<string> ExistingImages { get; set; } = new List<string>();

        public string? ImagesToDelete { get; set; } = string.Empty;
    }
}