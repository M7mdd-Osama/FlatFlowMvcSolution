using FlatFlow.DAL.Models.ApartmentModel;

namespace FlatFlow.DAL.Models.Shared
{
    public class ApartmentGroupPost : BaseEntity
    {
        public int ApartmentId { get; set; }
        public Apartment Apartment { get; set; }

        public int GroupId { get; set; }
        public FacebookGroup Group { get; set; }

        public bool IsPosted { get; set; }
    }
}
