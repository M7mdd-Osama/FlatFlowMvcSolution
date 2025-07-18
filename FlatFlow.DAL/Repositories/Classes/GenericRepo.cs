using FlatFlow.DAL.Data.DbContexts;
using FlatFlow.DAL.Models.Shared;
using FlatFlow.DAL.Repositories.Interfaces;

namespace FlatFlow.DAL.Repositories.Classes
{
    public class GenericRepo<TEntity>(AppDbContext _dbContext) : IGenericRepo<TEntity> where TEntity : BaseEntity
    {
        public int Add(TEntity entity)
        {
            _dbContext.Set<TEntity>().Add(entity);
            return _dbContext.SaveChanges();
        }

        public IEnumerable<TEntity> GetAll(bool withTracking = false)
        {
            if (withTracking)
                return _dbContext.Set<TEntity>().ToList();
            else
                return _dbContext.Set<TEntity>().AsNoTracking().ToList();
        }

        public TEntity? GetById(int id) => _dbContext.Set<TEntity>().Find(id);

        public int Remove(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
            return _dbContext.SaveChanges();
        }

        public int Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
            return _dbContext.SaveChanges();
        }
    }
}
