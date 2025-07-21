using FlatFlow.DAL.Data.DbContexts;
using FlatFlow.DAL.Repositories.Interfaces;

namespace FlatFlow.DAL.Repositories.Classes
{
    public class ClientRepo(AppDbContext dbContext) : GenericRepo<Client>(dbContext), IClientRepo
    {
        public IEnumerable<Client> GetClientsByUserId(string userId)
        {
            return dbContext.Clients.Where(c => c.UserId == userId);
        }
    }
}
