using FlatFlow.DAL.Models.ApartmentModel;
using FlatFlow.DAL.Models.Shared;

namespace FlatFlow.DAL.Models
{
    public class Client : BaseEntity
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public decimal? Commission { get; set; }

        public int ApartmentId { get; set; }
        public Apartment Apartment { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
