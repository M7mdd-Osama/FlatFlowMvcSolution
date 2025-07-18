using FlatFlow.DAL.Models.Shared;

namespace FlatFlow.DAL.Models.ApartmentModel
{
    public class ApartmentImage : BaseEntity
    {
        public string Url { get; set; }

        public int ApartmentId { get; set; }
        public Apartment Apartment { get; set; }
    }
}
