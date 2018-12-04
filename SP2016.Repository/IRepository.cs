namespace SP2016.Repository
{ 
    public interface IRepository<in TContext, TEntity> where TEntity : new()
    {
        /// <summary>
        /// Add new entity
        /// </summary>
        /// <param name="context">Node which contains the repository</param>
        /// <param name="entity">Entity to be added</param>
        void Add(TContext context, TEntity entity);

        /// <summary>
        /// Update an entity
        /// </summary>
        /// <param name="context">Node which contains the repository</param>
        /// <param name="entity">Entity to be updated</param>
        void Update(TContext context, TEntity entity);

        /// <summary>
        /// Get item by unique identificator.
        /// </summary>
        /// <param name="context">Node which contains the repository</param>
        /// <param name="id">Unique identificator</param>
        TEntity GetEntityById(TContext context, int id);

        /// <summary>
        /// Get collection of all existing entities
        /// </summary>
        /// <param name="context">Node which contains the repository</param>
        /// <param name="recursive">Include entities from sub-repositories</param>
        TEntity[] GetAllEntities(TContext context);

        /// <summary>
        /// Delete an entity
        /// </summary>
        /// <param name="context">Node which contains the repository</param>
        /// <param name="entity">Entity to be deleted</param>
        void Delete(TContext context, TEntity entity);
    }
}
