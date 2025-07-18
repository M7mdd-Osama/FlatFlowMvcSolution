using FlatFlow.DAL.Data.DbContexts;
using FlatFlow.DAL.Models.ApartmentModel;
using FlatFlow.DAL.Repositories.Interfaces;

namespace FlatFlow.DAL.Repositories.Classes
{
    public class ApartmentRepo(AppDbContext dbContext) : GenericRepo<Apartment>(dbContext), IApartmentRepo
    {
        public IEnumerable<Apartment> GetWithImagesAndClients()
        {
            return dbContext.Apartments
                .Include(a => a.ApartmentImages)
                .Include(a => a.Clients)
                .AsNoTracking()
                .ToList();
        }

        public IEnumerable<Apartment> Search(string term)
        {
            return dbContext.Apartments
                .Include(a => a.ApartmentImages)
                .Include(a => a.Clients)
                .Where(a => a.Title.Contains(term) || a.Location.Contains(term))
                .AsNoTracking()
            .ToList();
        }
    }
}
