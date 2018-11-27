using SP2016.Repository.Entities;

namespace SP2016.Repository
{
    public interface IBaseEntityRepository<in TContext, TEntity>
        : IBaseRepository<TContext, TEntity> where TEntity : IEntity, new()
    {
        TEntity GetEntityById(TContext context, int id);
    }
}
