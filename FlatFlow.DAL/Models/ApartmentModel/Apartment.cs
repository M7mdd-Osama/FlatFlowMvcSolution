using FlatFlow.DAL.Models.Shared;

namespace FlatFlow.DAL.Models.ApartmentModel
{
    public class Apartment : BaseEntity
    {
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public bool IsRented { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<ApartmentImage> ApartmentImages { get; set; }
        public ICollection<Client> Clients { get; set; }
        public ICollection<ApartmentGroupPost> ApartmentGroupPosts { get; set; }
    }
}
