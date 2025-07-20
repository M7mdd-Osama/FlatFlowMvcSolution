using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FlatFlow.PL.ViewModels
{
    public class AddApartmentViewModel
    {
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

        [Display(Name = "Apartment Images")]
        public List<IFormFile>? Images { get; set; }
    }
}