using FlatFlow.DAL.Data.DbContexts;
using FlatFlow.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlatFlow.DAL.Repositories.Classes
{
    public class ClientRepo(AppDbContext dbContext) : GenericRepo<Client>(dbContext), IClientRepo
    {
        public IQueryable<Client> GetAllWithApartments()
        {
            return dbContext.Clients.Include(c => c.Apartment);
        }

        public IEnumerable<Client> GetClientsByUserId(string userId)
        {
            return dbContext.Clients.Where(c => c.UserId == userId);
        }
    }
}
