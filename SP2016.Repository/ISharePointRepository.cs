using Microsoft.SharePoint;
using SP2016.Repository.Entities;

namespace SP2016.Repository
{
    public interface ISharePointRepository<T> : IBaseEntityRepository<SPWeb, T> where T : IEntity, new()
    {
        T CreateEntity(SPWeb web, SPItemEventProperties afterProperties);
        T CreateEntity(SPWeb web, SPListItem listItem);
        T CreateEntity(SPWeb web, SPListItemVersion listItemVersion);

        /// <summary>
        /// Добавить сущность, управляя срабатыванием приемников событий
        /// </summary>
        /// <param name="entity">Сущность для добавления</param>
        /// <param name="context">Узел, на который необходимо добавить сущность</param>
        /// <param name="eventFiringEnabled">true - если приемники событий должны срабатывать, false - в противном случае</param>
        void Add(SPWeb web, T entity, bool eventFiringEnabled);

        /// <summary>
        /// Добавить сущность в папку
        /// </summary>
        /// <param name="folderListRelativeUrl">Адрес папки, относительно корневой</param>
        /// <param name="entity">Сущность для добавления</param>
        /// <param name="context">Узел, на который необходимо добавить сущность</param>
        void Add(SPWeb web, string folderListRelativeUrl, T entity);

        /// <summary>
        /// Добавить сущность в папку, управляя срабатыванием приемников событий
        /// </summary>
        /// <param name="folderListRelativeUrl">Адрес папки, относительно корневой</param>
        /// <param name="entity">Сущность для добавления</param>
        /// <param name="context">Узел, на который необходимо добавить сущность</param>
        /// <param name="eventFiringEnabled">true - если приемники событий должны срабатывать, false - в противном случае</param>
        void Add(SPWeb web, string folderListRelativeUrl, T entity, bool eventFiringEnabled);

        /// <summary>
        /// Обновить сущность
        /// </summary>
        /// <param name="entity">Сущность для обновления</param>
        /// <param name="context">Узел, на который необходимо добавить сущность</param>
        /// <param name="trackChanges">Позволить ли SharePoint отслеживать изменения</param>
        void Update(SPWeb web, T entity, bool trackChanges);

        /// <summary>
        /// Получение коллекции сущностей
        /// </summary>
        /// <param name="expr">Выражение для фильтрации</param>
        /// <param name="context">Узел, с которого необходимо получить коллекцию</param>
        /// <param name="recursive">Получить элементы рекурсивно из папок</param>
        /// <returns>Все сущности, удовлетворяющие запросу</returns>
        T[] GetEntities(SPWeb web, Caml.IExpression expr, bool recursive, uint rowLimit);

        string GetDisplayFormUrl(SPWeb web, T entity);
    }
}
