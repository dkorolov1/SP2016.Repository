namespace SP2016.Repository
{ 
    public interface IBaseRepository<in TContext, TEntity> where TEntity : new()
    {
        /// <summary>
        /// Добавить сущность
        /// </summary>
        /// <param name="entity">Сущность для добавления</param>
        /// <param name="context">Узел, на который необходимо добавить сущность</param>
        void Add(TContext context, TEntity entity);

        /// <summary>
        /// Обновить сущность
        /// </summary>
        /// <param name="entity">Сущность для обновления</param>
        /// <param name="context">Узел, на который необходимо добавить сущность</param>
        void Update(TContext context, TEntity entity);

        /// <summary>
        /// Получение коллекции сущностей
        /// </summary>
        /// <param name="expr">Выражение для фильтрации</param>
        /// <param name="context">Узел, с которого необходимо получить коллекцию</param>
        /// <returns>Все сущности, удовлетворяющие запросу</returns>
        TEntity[] GetEntities(TContext context, object expr, uint rowLimit);

        /// <summary>
        /// Получение коллекции сущностей
        /// </summary>
        /// <param name="context">Узел, с которого необходимо получить коллекцию</param>
        /// <returns>Все сущности, удовлетворяющие запросу</returns>
        TEntity[] GetAllEntities(TContext context, bool recursive);

        /// <summary>
        /// Удалить сущность
        /// </summary>
        /// <param name="context">Узел, в котором необходимо удалить сущности</param>
        /// <param name="entity">Удаляемая сущность</param>
        void Delete(TContext context, TEntity entity);
    }
}
