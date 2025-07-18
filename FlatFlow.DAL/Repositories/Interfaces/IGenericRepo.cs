using FlatFlow.DAL.Models.Shared;

namespace FlatFlow.DAL.Repositories.Interfaces
{
    public interface IGenericRepo<TEntity> where TEntity : BaseEntity
    {
        int Add(TEntity entity);
        IEnumerable<TEntity> GetAll(bool withTracking = false);
        TEntity? GetById(int id);
        int Remove(TEntity entity);
        int Update(TEntity entity);
    }
}
