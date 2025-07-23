using Demo.BLL.Interfaces;
using FlatFlow.DAL.Data.DbContexts;
using FlatFlow.DAL.Repositories.Classes;
using FlatFlow.DAL.Repositories.Interfaces;

namespace Demo.BLL.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _dbContext;

        public IApartmentGroupPostRepo ApartmentGroupPostRepo { get; set; }
        public IApartmentRepo ApartmentRepo { get; set; }
        public IClientRepo ClientRepo { get; set; }

        public UnitOfWork(AppDbContext dbContext)
        {
            ApartmentGroupPostRepo = new ApartmentGroupPostRepo(dbContext);
            ApartmentRepo = new ApartmentRepo(dbContext);
            ClientRepo = new ClientRepo(dbContext);
            _dbContext = dbContext;
        }

        public async Task<int> CompleteAsync()
        => await _dbContext.SaveChangesAsync();

        public void Dispose()
        => _dbContext.Dispose();

    }
}