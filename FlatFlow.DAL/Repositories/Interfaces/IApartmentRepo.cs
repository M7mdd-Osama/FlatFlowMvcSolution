using FlatFlow.DAL.Models.ApartmentModel;

namespace FlatFlow.DAL.Repositories.Interfaces
{
    public interface IApartmentRepo : IGenericRepo<Apartment>
    {
        IEnumerable<Apartment> GetWithImagesAndClients();
        IEnumerable<Apartment> Search(string term);
        void RemoveImages(List<ApartmentImage> images);
    }
}
