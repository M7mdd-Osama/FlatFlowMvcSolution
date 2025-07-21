using FlatFlow.DAL.Models.ApartmentModel;
using System.ComponentModel.DataAnnotations;

namespace FlatFlow.PL.ViewModels
{
    public class EditClientViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } = "Active";

        [StringLength(500, ErrorMessage = "Note cannot exceed 500 characters")]
        public string? Note { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Commission must be a positive number")]
        public decimal? Commission { get; set; }

        public int? ApartmentId { get; set; }

        public List<Apartment> Apartments { get; set; } = new List<Apartment>();
    }
}
