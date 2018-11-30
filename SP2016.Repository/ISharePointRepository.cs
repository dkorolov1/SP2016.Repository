using Microsoft.SharePoint;
using SP2016.Repository.Entities;

namespace SP2016.Repository
{
    public interface ISharePointRepository<T> : IRepository<SPWeb, T> where T : IEntity, new()
    {
        T CreateEntity(SPWeb web, SPItemEventProperties afterProperties);
        T CreateEntity(SPWeb web, SPListItem listItem);
        T CreateEntity(SPWeb web, SPListItemVersion listItemVersion);

        /// <summary>
        /// Adding entity with event firing configuration
        /// </summary>
        /// <param name="web">Web which contains the list</param>
        /// <param name="entity">Entity to be added</param>
        /// <param name="eventFiringEnabled">Should addition events be fired</param>
        void Add(SPWeb web, T entity, bool eventFiringEnabled);

        /// <summary>
        /// Adding entity to a folder
        /// </summary>
        /// <param name="web">Web which contains the list</param>
        /// <param name="folderListRelativeUrl">List-relative folder URL</param>
        /// <param name="entity">Entity to be added</param>
        void Add(SPWeb web, string folderListRelativeUrl, T entity);

        /// <summary>
        /// Adding entity to a folder with event firing configuration
        /// </summary>
        /// <param name="web">Web which contains the list</param>
        /// <param name="folderListRelativeUrl">List-relative folder URL</param>
        /// <param name="entity">Entity to be added</param>
        /// <param name="eventFiringEnabled">Should addition events be fired</param>
        void Add(SPWeb web, string folderListRelativeUrl, T entity, bool eventFiringEnabled);

        /// <summary>
        /// Updating an entity
        /// </summary>
        /// <param name="web">Web which contains the list</param>
        /// <param name="entity">Entity to be updated</param>
        /// <param name="trackChanges">Allow SharePoint to track changes</param>
        void Update(SPWeb web, T entity, bool trackChanges);

        /// <summary>
        /// Getting collection of enities with query
        /// </summary>
        /// <param name="web">Web which contains the list</param>
        /// <param name="expr">Expression for filtering</param>
        /// <param name="recursive">Get entities from all folders</param>
        /// <returns>Все сущности, удовлетворяющие запросу</returns>
        T[] GetEntities(SPWeb web, Caml.IExpression expr, bool recursive, uint rowLimit);

        string GetDisplayFormUrl(SPWeb web, T entity);
    }
}
