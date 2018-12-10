namespace SP2016.Repository
{ 
    public interface IRepository<in TContext, TEntity> where TEntity : new()
    {
        /// <summary>
        /// Add a new entity
        /// </summary>
        /// <param name="entity">Entity to be added</param>
        void Add(TContext context, TEntity entity);

        /// <summary>
        /// Update an existing entity
        /// </summary>
        /// <param name="entity">Entity to be updated</param>
        void Update(TContext context, TEntity entity);

        /// <summary>
        /// Get item by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        TEntity GetEntityById(TContext context, int id);

        /// <summary>
        /// Get collection of all existing entities
        /// </summary>
        /// <param name="recursive">Include entities from sub-repositories</param>
        TEntity[] GetAllEntities(TContext context);

        /// <summary>
        /// Delete an entity
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        void Delete(TContext context, TEntity entity);
    }
}
