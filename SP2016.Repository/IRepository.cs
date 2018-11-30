namespace SP2016.Repository
{ 
    public interface IRepository<in TContext, TEntity> where TEntity : new()
    {
        /// <summary>
        /// Add a new entity
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="entity">Entity to be added</param>
        void Add(TContext context, TEntity entity);

        /// <summary>
        /// Update an existing entity
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="entity">Entity to be updated</param>
        void Update(TContext context, TEntity entity);

        /// <summary>
        /// Get item by identifier
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="id">identifier</param>
        TEntity GetEntityById(TContext context, int id);

        /// <summary>
        /// Get entities by query
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="expr">Query expression</param>
        /// <param name="rowLimit">Max number of entities to be returned</param>
        TEntity[] GetEntities(TContext context, object expr, uint rowLimit);

        /// <summary>
        /// Get all entities
        /// </summary>
        /// <param name="context">Node which contains the repository</param>
        /// <param name="recursive">Include entities from sub-repositories</param>
        TEntity[] GetAllEntities(TContext context, bool recursive);

        /// <summary>
        /// Delete an entity
        /// </summary>
        /// <param name="context">Node which contains the repository</param>
        /// <param name="entity">Entity to be deleted</param>
        void Delete(TContext context, TEntity entity);
    }
}
