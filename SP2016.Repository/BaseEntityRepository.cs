using System;
using System.IO;
using System.Linq;
using Microsoft.SharePoint;
using System.Globalization;
using SP2016.Repository.Caml;
using SP2016.Repository.Utils;
using SP2016.Repository.Enums;
using SP2016.Repository.Mapping;
using SP2016.Repository.Service;
using System.Collections.Generic;
using SP2016.Repository.Entities;
using SP2016.Repository.Repository;
using Microsoft.SharePoint.Utilities;

namespace SP2016.Repository
{
    /// <summary>
    /// The BaseEntityRepositroy is the base class for all repository classes
    /// used for accessing lists in SharePoint. The base class is used to enforce
    /// the repository pattern for accessing lists in custom code. It abstracts
    /// the SharePoint calls from custom code.
    /// </summary>
    /// <typeparam name="TEntity">Конкретная сущность, для которой создается репозиторий</typeparam>
    public abstract class BaseEntityRepository<TEntity> : ISharePointRepository<TEntity> where TEntity : BaseEntity, new()
    {
        protected virtual FieldToEntityPropertyMapping[] FieldMappings => new FieldToEntityPropertyMapping[0];
        public abstract string ListName { get; }

        private readonly SPFieldToPropertyMapper ListItemFieldMapper;
        private readonly FieldToEntityPropertyMapping[] DefaultFieldMappings = new FieldToEntityPropertyMapping[]
        {
            new FieldToEntityPropertyMapping("ID", true),
            new FieldToEntityPropertyMapping("GUID", true),
            new FieldToEntityPropertyMapping("Author", true),
            new FieldToEntityPropertyMapping("Editor", true),
            new FieldToEntityPropertyMapping("Modified", true),
            new FieldToEntityPropertyMapping("Created", true),
            new FieldToEntityPropertyMapping("FileLeafRef", true)
        };
        
        public BaseEntityRepository()
        {
            var intersectedMappings = FieldMappings
                .Intersect(DefaultFieldMappings, new FieldToEntityPropertyMappingComparer())
                .ToArray();

            if (intersectedMappings.Length > 0) {
                string intersectedMappingsFieldsStr = string.Join(",", intersectedMappings.Select(f => f.FieldName));
                throw new ApplicationException($"You can't map default fields. ({intersectedMappingsFieldsStr})");
            }

            var allFieldMappings = FieldMappings
                .Union(DefaultFieldMappings)
                .ToArray();

            ListItemFieldMapper = new SPFieldToPropertyMapper(allFieldMappings);
        }

        #region Получение всех сущностей

        /// <summary>
        /// Получить все сущности
        /// </summary>
        /// <param name="web">Узел</param>
        /// <returns>Массив сущностей</returns>
        public TEntity[] GetAllEntities(SPWeb web, bool recursive = true)
        {
            CamlQueryBuilder builder = new CamlQueryBuilder();
            SPQuery query = builder.Build();
            return (recursive) ? GetEntities(web, query.Query, recursive) : GetEntities(web, query);
        }

        #endregion

        #region Фильтры

        /// <summary>
        /// Получение коллекции сущностей
        /// </summary>
        /// <param name="caml">Caml запрос</param>
        /// <param name="web">Узел, с которого необходимо получить коллекцию</param>
        /// <param name="recursive">Получить элементы рекурсивно из всех папок</param>
        /// <returns>Все сущности, удовлетворяющие запросу</returns>
        private TEntity[] GetEntities(SPWeb web, string caml, bool recursive, uint rowLimit = 0)
        {
            var query = new SPQuery
            {
                Query = caml
            };

            if (recursive)
                query.ViewAttributes = "Scope=\"Recursive\"";
            if (rowLimit > 0)
                query.RowLimit = rowLimit;
            return GetEntities(web, query);
        }

            #region Специфичные фильтры

        /// <summary>
        /// Получить сущности по названию
        /// </summary>
        /// <param name="web">Веб-сайт</param>
        /// <param name="title">Название сущности (Поле Title, Название)</param>
        /// <returns>Сущности с названием title</returns>
        public virtual TEntity[] GetEntitiesByTitle(SPWeb web, string title, bool recursive = true, uint rowLimit = 0)
        {
            Filter eqTitleFilter = new Filter(FilterType.Equal, "Title", title, FilterValueType.Text);
            Query query = new Query() { Where = eqTitleFilter, Recursive = recursive };

            SimpleCamlBuilder camlBuilder = new SimpleCamlBuilder();
            string camlQuery = camlBuilder.BuildCaml(query);
            return GetEntities(web, camlQuery, recursive, rowLimit);
        }

        /// <summary>
        /// Получить сущность по названию
        /// </summary>
        /// <param name="web">Веб-сайт</param>
        /// <param name="title">Название сущности (Поле Title, Название)</param>
        /// <returns>Сущности с названием title</returns>
        public virtual TEntity GetEntityByTitle(SPWeb web, string title)
        {
            Filter eqTitleFilter = new Filter(FilterType.Equal, "Title", title, FilterValueType.Text);
            Query query = new Query() { Where = eqTitleFilter, Recursive = true };

            SimpleCamlBuilder camlBuilder = new SimpleCamlBuilder();
            string camlQuery = camlBuilder.BuildCaml(query);
            return GetEntities(web, camlQuery, true, 1).FirstOrDefault();
        }

            #endregion

        /// <summary>
        /// Получение элемента списка
        /// </summary>
        /// <param name="caml">Caml запрос для получения сущности</param>
        /// <param name="web">Узел, с которого необходимо получить сущность</param>
        /// <returns>Сущность с заполненными свойствами</returns>
        /// <remarks>Если caml запрос вернет несколько сущностей, будет взята первая из них</remarks>
        public TEntity GetEntity(SPWeb web, string caml)
        {
            TEntity entity = default(TEntity);

            var query = new SPQuery
            {
                Query = caml
            };

            SPListItem item = null;
            SPListItemCollection collection = null;

            collection = web.Lists[ListName].GetItems(query);

            if (collection != null && collection.Count > 0)
            {
                item = collection[0];
            }

            if (item != null)
            {
                entity = CreateEntity(web, item);
            }

            return entity;
        }

        /// <summary>
        /// Получить сущности из папки
        /// </summary>
        /// <param name="web">Веб-сайт</param>
        /// <param name="folderPath">Путь к папкам, создающим иерархию. Например folder1/folder2/folder3</param>
        /// <returns>Сущности</returns>
        public TEntity[] GetEntities(SPWeb web, string folderPath)
        {
            var list = web.Lists[ListName];
            var url = SPUtility.ConcatUrls(list.RootFolder.ServerRelativeUrl, folderPath);

            var folderService = new FolderService();
            var folder = folderService.GetFolderByUrl(web, list, folderPath);

            if (folder == null)
            {
                return new TEntity[0];
            }

            var query = new SPQuery { Folder = folder };
            return GetEntities(web, query);
        }

        /// <summary>
        /// Получить сущности по запросу
        /// </summary>
        /// <param name="web">Веб-сайт</param>
        /// <param name="query">Запрос</param>
        /// <returns>Сущности</returns>
        public TEntity[] GetEntities(SPWeb web, Query query, uint rowLimit = 0)
        {
            var camlBuilder = new RepositoryCamlBuilder<TEntity>
            {
                FieldCollection = web.Lists[ListName].Fields,
                ListItemFieldMapper = ListItemFieldMapper
            };

            string caml = camlBuilder.BuildCaml(query);
            return GetEntities(web, caml, query.Recursive, rowLimit);
        }

        /// <summary>
        /// Получение коллекции сущностей
        /// </summary>
        /// <param name="expr">Выражение для фильтрации</param>
        /// <param name="web">Узел, с которого необходимо получить коллекцию</param>
        /// <param name="recursive">Получить элементы рекурсивно из папок</param>
        /// <returns>Все сущности, удовлетворяющие запросу</returns>
        public TEntity[] GetEntities(SPWeb web, IExpression expr, bool recursive, uint rowLimit = 0)
        {
            var query = new Query
            {
                Where = expr,
                Recursive = recursive
            };

            return GetEntities(web, query, rowLimit);
        }

        /// <summary>
        /// Получение коллекции сущностей
        /// </summary>
        /// <param name="expr">Выражение для фильтрации</param>
        /// <param name="web">Узел, с которого необходимо получить коллекцию</param>
        /// <param name="recursive">Получить элементы рекурсивно из папок</param>
        /// <returns>Все сущности, удовлетворяющие запросу</returns>
        public TEntity[] GetEntities(SPWeb web, object expr, uint rowLimit = 0)
        {
            var query = new Query
            {
                Where = (IExpression)expr,
                Recursive = true
            };

            return GetEntities(web, query, rowLimit);
        }

        /// <summary>
        /// Получение коллекции сущностей
        /// </summary>
        /// <param name="query">Объект SPQuery, содержащий запрос</param>
        /// <param name="web">Узел, с которого необходимо получить коллекцию</param>
        /// <returns>Все сущности, удовлетворяющие запросу</returns>
        public TEntity[] GetEntities(SPWeb web, SPQuery query)
        {
            var collection = web.Lists[ListName].GetItems(query);

            return collection
                .Cast<SPListItem>()
                .Select(item => CreateEntity(web, item))
                .ToArray();
        }

        #endregion

        #region Создание сущностей из объектов SharePoint

        /// <summary>
        /// Заполнить сущность значениями элемента списка
        /// </summary>
        /// <param name="item">Элемент списка</param>
        /// <returns>Сущность с заполненными свойствами</returns>
        public virtual TEntity CreateEntity(SPWeb web, SPListItem spListItem)
        {
            TEntity entity = new TEntity();
            Type entityType = typeof(TEntity);

            ListItemFieldMapper.FillEntityFromSPListItem(web, entity, entityType, spListItem);
            entity.ListItem = spListItem;

            return entity;
        }


        public TEntity CreateEntity(SPWeb web, SPItemEventProperties properties)
        {
            return CreateEntity(web, properties.ListItem);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="web"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public virtual TEntity CreateEntityFromAfterProperties(SPWeb web, SPItemEventProperties properties)
        {
            TEntity entity = new TEntity();
            Type entityType = typeof(TEntity);

            ListItemFieldMapper.FillEntityFromAfterProperties(web, entity, entityType, properties);
            entity.ListItem = properties.ListItem;
            return entity;
        }

        /// <summary>
        /// Заполнить сущность значениями элемента списка
        /// </summary>
        /// <param name="item">Элемент списка</param>
        /// <returns>Сущность с заполненными свойствами</returns>
        public virtual TEntity CreateEntity(SPWeb web, SPListItemVersion spListItem)
        {
            TEntity entity = new TEntity();
            Type entityType = typeof(TEntity);

            ListItemFieldMapper.FillEntityFromSPListItem(web, entity, entityType, spListItem);
            entity.ListItem = spListItem.ListItem;
            return entity;
        }

        #endregion

        #region Последние сущности

        /// <summary>
        /// Получить последнюю сущность
        /// </summary>
        /// <param name="web">Узел, с которого необходимо получить сущность</param>
        /// <returns></returns>
        public TEntity GetLastEntity(SPWeb web)
        {
            Query query = new Query();
            query.OrderBy.Add(new FieldReference("ID", SortOrder.Descending));

            return GetEntities(web, query, 1).FirstOrDefault();
        }

        /// <summary>
        /// Получить ID последней сущности или 0
        /// </summary>
        /// <param name="web">Узел, с которого необходимо получить сущность</param>
        /// <returns></returns>
        public int GetLastEntityId(SPWeb web)
        {
            TEntity entity = GetLastEntity(web);

            if (entity != null)
                return entity.ID;
            else
                return 0;
        }

        #endregion

        #region Получение сущностей по идентификаторам

        protected SPListItem GetListItemById(SPWeb web, int id)
        {
            try
            {
                return web.Lists[this.ListName].GetItemById(id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Получить сущность по уникальному идентификатору
        /// </summary>
        /// <param name="web">Веб-сайт</param>
        /// <param name="uniqueId">Уникальный идентификатор элемента списка</param>
        /// <returns>Возвращает сущность или null</returns>
        public TEntity GetEntityByUniqueId(SPWeb web, Guid uniqueId)
        {
            SPListItem item = web.Lists[ListName].GetItemByUniqueId(uniqueId);
            if (null != item)
                return this.CreateEntity(web, item);
            else
                return null;
        }

        /// <summary>
        /// Получить сущность по идентификатору
        /// </summary>
        /// <param name="web">Веб-сайт</param>
        /// <param name="guid">Идентификатор элемента списка</param>
        /// <returns>Возвращает сущность или null</returns>
        public TEntity GetEntityByGUID(SPWeb web, Guid guid)
        {
            Filter eqGUIDFilter = new Filter(FilterType.Equal, "GUID", guid, FilterValueType.GUID);
            Query query = new Query() { Where = eqGUIDFilter, Recursive = true };

            SimpleCamlBuilder camlBuilder = new SimpleCamlBuilder();
            string camlQuery = camlBuilder.BuildCaml(query);
            return GetEntities(web, camlQuery, true, 1).FirstOrDefault();
        }

        /// <summary>
        /// Получить сущность по ИД (null если сущность не найдена)
        /// </summary>
        /// <param name="id">ИД сущности</param>
        /// <param name="web">Узел, с которого необходимо получить сущность</param>
        /// <returns>Возвращает сущность или null</returns>
        public TEntity GetEntityById(SPWeb web, int id)
        {
            SPListItem item = GetListItemById(web, id);
            if (null != item)
                return this.CreateEntity(web, item);
            else
                return null;
        }

        /// <summary>
        /// Получить сущность по ИД
        /// </summary>
        /// <param name="web">Узел, с которого необходимо получить сущность</param>
        /// <param name="lookupValue">Подстановочное значение для сущности</param>
        /// <returns>Сущность</returns>
        public TEntity GetEntityById(SPWeb web, SPFieldLookupValue lookupValue)
        {
            if (lookupValue == null || lookupValue.LookupId == 0) return null;
            return GetEntityById(web, lookupValue.LookupId);
        }

        /// <summary>
        /// Получить сущности по их идентификаторам
        /// </summary>
        /// <param name="web">Веб-сайт</param>
        /// <param name="ids">Идентификаторы сущностей</param>
        /// <returns>Сущности</returns>
        public TEntity[] GetEntitiesByIds(SPWeb web, int[] ids)
        {
            if (ids == null || ids.Length == 0) return new TEntity[0];

            int[][] chunks = ids
                                .Select((id, i) => new { Value = id, Index = i })
                                .GroupBy(x => x.Index / 200)
                                .Select(grp => grp.Select(x => x.Value).ToArray())
                                .ToArray();

            List<TEntity> result = new List<TEntity>(ids.Length);

            foreach (int[] chunk in chunks)
            {
                Filter filter = new Filter(FilterType.In, "ID", chunk, FilterValueType.Lookup);
                TEntity[] entities = GetEntities(web, filter, true);
                result.AddRange(entities);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Получить сущности по их идентификаторам
        /// </summary>
        /// <param name="web">Веб-сайт</param>
        /// <param name="lookups">Коллекция подстановочных идентификаторов сущностей</param>
        /// <returns>Сущности</returns>
        public TEntity[] GetEntitiesByIds(SPWeb web, SPFieldLookupValueCollection lookups)
        {
            if (lookups == null) return new TEntity[0];

            int[] ids = lookups.Select(lookup => lookup.LookupId).ToArray();
            return GetEntitiesByIds(web, ids);
        }

        /// <summary>
        /// Получить сущности по их идентификаторам
        /// </summary>
        /// <param name="web">Веб-сайт</param>
        /// <param name="lookups">Коллекция подстановочных идентификаторов сущностей</param>
        /// <returns>Сущности</returns>
        public TEntity[] GetEntitiesByIds(SPWeb web, SPFieldLookupValue[] lookups)
        {
            if (lookups == null) return new TEntity[0];

            int[] ids = lookups.Select(lookup => lookup.LookupId).ToArray();
            return GetEntitiesByIds(web, ids);
        }

        #endregion
        
        #region Добавление элементов

        /// <summary>
        /// Добавление нового элемента
        /// </summary>
        /// <param name="entity">Сущность с данными для добавления</param>
        /// <param name="web">Узел, на который необходимо добавить информацию</param>
        /// <returns>Идентификатор созданного элемента</returns>
        private void AddListItem(SPWeb web, TEntity entity)
        {
            SPListItem newItem = web.Lists[this.ListName].Items.Add();
            UpdateListItemInternal(web, entity, newItem);

            ListItemFieldMapper.FillEntityFromSPListItem(web, entity, typeof(TEntity), newItem);
        }

        /// <summary>
        /// Добавить сущность
        /// </summary>
        /// <param name="entity">Сущность для добавления</param>
        /// <param name="web">Узел, на который необходимо добавить сущность</param>
        public virtual void Add(SPWeb web, TEntity entity)
        {
            AddListItem(web, entity);
            entity.ListItem = GetList(web).GetItemById(entity.ID);
        }

        /// <summary>
        /// Добавить сущность, управляя срабатыванием приемников событий
        /// </summary>
        /// <param name="entity">Сущность для добавления</param>
        /// <param name="web">Узел, на который необходимо добавить сущность</param>
        /// <param name="eventFiringEnabled">true - если приемники событий должны срабатывать, false - в противном случае</param>
        public void Add(SPWeb web, TEntity entity, bool eventFiringEnabled)
        {
            if (!eventFiringEnabled)
            {
                using (new DisabledItemEventsScope())
                    Add(web, entity);
            }
            else
                Add(web, entity);
        }

        /// <summary>
        /// Добавить сущность в папку
        /// </summary>
        /// <param name="folderListRelativeUrl">Адрес папки, относительно корневой</param>
        /// <param name="entity">Сущность для добавления</param>
        /// <param name="web">Узел, на который необходимо добавить сущность</param>
        public virtual void Add(SPWeb web, string folderListRelativeUrl, TEntity entity)
        {
            AddListItemToFolder(web, folderListRelativeUrl, entity);
            entity.ListItem = GetList(web).GetItemById(entity.ID);
        }

        /// <summary>
        /// Добавить сущность в папку, управляя срабатыванием приемников событий
        /// </summary>
        /// <param name="folderListRelativeUrl">Адрес папки, относительно корневой</param>
        /// <param name="entity">Сущность для добавления</param>
        /// <param name="web">Узел, на который необходимо добавить сущность</param>
        /// <param name="eventFiringEnabled">true - если приемники событий должны срабатывать, false - в противном случае</param>
        public void Add(SPWeb web, string folderListRelativeUrl, TEntity entity, bool eventFiringEnabled)
        {
            if (!eventFiringEnabled)
            {
                using (new DisabledItemEventsScope())
                    Add(web, folderListRelativeUrl, entity);
            }
            else
                Add(web, folderListRelativeUrl, entity);
        }

        /// <summary>
        /// Добавить несколько сущностей
        /// </summary>
        /// <param name="entity">Сущность для добавления</param>
        /// <param name="web">Узел, на который необходимо добавить сущность</param>
        public virtual void AddRange(SPWeb web, IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                AddListItem(web, entity);
                entity.ListItem = this.GetList(web).GetItemById(entity.ID);
            }
        }

        private void AddListItemToFolder(SPWeb web, string folderListRelativeUrl, TEntity entity)
        {
            SPList list = web.Lists[this.ListName];
            string url = SPUtility.ConcatUrls(list.RootFolder.ServerRelativeUrl, folderListRelativeUrl);

            FolderService folderService = new FolderService();
            SPFolder folder = folderService.GetFolderByUrl(web, list, folderListRelativeUrl);
            if (folder == null)
            {
                folderService.CreatePath(web, list, folderListRelativeUrl);
            }

            SPListItem newItem = list.Items.Add(url, SPFileSystemObjectType.File, null);
            UpdateListItemInternal(web, entity, newItem);
            ListItemFieldMapper.FillEntityFromSPListItem(web, entity, typeof(TEntity), newItem);
        }

        /// <summary>
        /// Добавить несколько сущностей
        /// </summary>
        /// <param name="entitiesWithFolderPath">Сущность для добавления с указанием папки</param>
        /// <param name="web">Узел, на который необходимо добавить сущность</param>
        public virtual void AddRange(SPWeb web, EntityContainer<TEntity>[] entitiesWithFolderPath)
        {
            foreach (var withFolderPath in entitiesWithFolderPath)
            {
                AddListItemToFolder(web, withFolderPath.FolderPath, withFolderPath.Entity);
                withFolderPath.Entity.ListItem = this.GetList(web).GetItemById(withFolderPath.Entity.ID);
            }
        }

        /// <summary>
        /// Добавить несколько сущностей, управляя срабатыванием приемников событий
        /// </summary>
        /// <param name="entity">Сущность для добавления</param>
        /// <param name="web">Узел, на который необходимо добавить сущность</param>
        /// <param name="eventFiringEnabled">true - если приемники событий должны срабатывать, false - в противном случае</param>
        public void AddRange(SPWeb web, IEnumerable<TEntity> entities, bool eventFiringEnabled)
        {
            if (!eventFiringEnabled)
            {
                using (new DisabledItemEventsScope())
                    AddRange(web, entities);
            }
            else
                AddRange(web, entities);
        }

        /// <summary>
        /// Добавить несколько сущностей, управляя срабатыванием приемников событий
        /// </summary>
        /// <param name="entity">Сущность для добавления</param>
        /// <param name="web">Узел, на который необходимо добавить сущность</param>
        /// <param name="eventFiringEnabled">true - если приемники событий должны срабатывать, false - в противном случае</param>
        public void AddRange(SPWeb web, EntityContainer<TEntity>[] entitiesWithFolderPath, bool eventFiringEnabled)
        {
            if (!eventFiringEnabled)
            {
                using (new DisabledItemEventsScope())
                    AddRange(web, entitiesWithFolderPath);
            }
            else
                AddRange(web, entitiesWithFolderPath);
        }

        /// <summary>
        /// Массовое добавление сущностей
        /// </summary>
        /// <param name="entities">Сущности</param>
        /// <param name="web">Веб-узел</param>
        /// <param name="blocksize">Количество удаляемых сущностей в рамках одного запроса к БД</param>
        /// <param name="batchFinishedFunc">Процедура, выполняемая после добавления блока элементов. Принимает количество добавленных элементов</param>
        public void AddBatch(TEntity[] entities, SPWeb web, int blocksize = 1000, Action<int> batchFinishedFunc = null)
        {
            string command = "<Method ID=\"{0}\"><SetList>{1}</SetList><SetVar Name=\"ID\">New</SetVar><SetVar Name=\"Cmd\">Save</SetVar>{3}</Method>";
            BatchUtil.ProcessBatch(entities, web, ListName, ListItemFieldMapper, command, blocksize);
        }

        /// <summary>
        /// Массовое добавление сущностей, управляя срабатыванием приемников событий
        /// </summary>
        /// <param name="entities">Сущности</param>
        /// <param name="web">Веб-узел</param>
        /// <param name="eventFiringEnabled">true - если приемники событий должны срабатывать, false - в противном случае</param>
        /// <param name="blocksize">Количество удаляемых сущностей в рамках одного запроса к БД</param>
        /// <param name="batchFinishedFunc">Процедура, выполняемая после добавления блока элементов. Принимает количество добавленных элементов</param>
        public void AddBatch(TEntity[] entities, SPWeb web, bool eventFiringEnabled, int blocksize = 1000, Action<int> batchFinishedFunc = null)
        {
            if (!eventFiringEnabled)
            {
                using (new DisabledItemEventsScope())
                    AddBatch(entities, web, blocksize, batchFinishedFunc);
            }
            else
                AddBatch(entities, web, blocksize, batchFinishedFunc);
        }

        /// <summary>
        /// Массовое добавление сущностей с учетом структуры папок
        /// </summary>
        /// <param name="entitiesWithFolderPath">Сущности с указанием папок</param>
        /// <param name="web">Веб-узел</param>
        /// <param name="blocksize">Количество удаляемых сущностей в рамках одного запроса к БД</param>
        /// <param name="batchFinishedFunc">Процедура, выполняемая после добавления блока элементов. Принимает количество добавленных элементов</param>
        public void AddBatch(EntityContainer<TEntity>[] entitiesWithFolderPath, SPWeb web, int blocksize = 1000, Action<int> batchFinishedFunc = null)
        {
            string command = "<Method ID=\"{0}\"><SetList>{1}</SetList><SetVar Name=\"RootFolder\">{2}</SetVar><SetVar Name=\"ID\">New</SetVar><SetVar Name=\"Cmd\">Save</SetVar>{3}</Method>";
            SPList list = web.Lists[ListName];

            Func<int, string> methodExtractor = index =>
            {
                string fields = ListItemFieldMapper.GenerateBatchCommandFromEntity(web, list, typeof(TEntity), entitiesWithFolderPath[index].Entity);
                string folderServerRelativeUrl = SPUtility.ConcatUrls(list.RootFolder.ServerRelativeUrl, entitiesWithFolderPath[index].FolderPath);
                return string.Format(command, index, list.ID, folderServerRelativeUrl, fields);
            };

            BatchUtil.ProcessBatch(entitiesWithFolderPath.Length, methodExtractor, web, blocksize, batchFinishedFunc);
        }

        /// <summary>
        /// Массовое добавление сущностей с учетом структуры папок, управляя срабатыванием приемников событий
        /// </summary>
        /// <param name="entitiesWithFolderPath">Сущности с указанием папок</param>
        /// <param name="web">Веб-узел</param>
        /// <param name="eventFiringEnabled">true - если приемники событий должны срабатывать, false - в противном случае</param>
        /// <param name="blocksize">Количество удаляемых сущностей в рамках одного запроса к БД</param>
        /// <param name="batchFinishedFunc">Процедура, выполняемая после добавления блока элементов. Принимает количество добавленных элементов</param>
        public void AddBatch(EntityContainer<TEntity>[] entitiesWithFolderPath, SPWeb web, bool eventFiringEnabled, int blocksize = 1000, Action<int> batchFinishedFunc = null)
        {
            if (!eventFiringEnabled)
            {
                using (new DisabledItemEventsScope())
                    AddBatch(entitiesWithFolderPath, web, blocksize, batchFinishedFunc);
            }
            else
                AddBatch(entitiesWithFolderPath, web, blocksize, batchFinishedFunc);
        }

        #endregion

        #region Удаление элементов

        /// <summary>
        /// Удалить все сущности
        /// </summary>      
        /// <param name="web">Узел, в котором необходимо удалить сущности</param>
        public void DeleteAll(SPWeb web)
        {
            TEntity[] entities = GetAllEntities(web);
            DeleteBatch(entities, web);
        }

        /// <summary>
        /// Удалить все сущности, управляя срабатыванием приемников событий
        /// </summary>      
        /// <param name="web">Узел, в котором необходимо удалить сущности</param>
        /// <param name="eventFiringEnabled">true - если приемники событий должны срабатывать, false - в противном случае</param>
        public void DeleteAll(SPWeb web, bool eventFiringEnabled)
        {
            if (!eventFiringEnabled)
            {
                using (new DisabledItemEventsScope())
                    DeleteAll(web);
            }
            else
                DeleteAll(web);
        }

        /// <summary>
        /// Массовое удаление сущностей
        /// </summary>
        /// <param name="entities">Сущности</param>
        /// <param name="web">Веб-узел</param>
        /// <param name="blocksize">Количество удаляемых сущностей в рамках одного запроса к БД</param>
        /// <param name="batchFinishedFunc">Процедура, выполняемая после удаления блока элементов. Принимает количество удалённых элементов</param>
        public void DeleteBatch(TEntity[] entities, SPWeb web, int blocksize = 1000, Action<int> batchFinishedFunc = null)
        {
            string command = "<Method><SetList Scope=\"Request\">{1}</SetList><SetVar Name=\"ID\">{2}</SetVar><SetVar Name=\"Cmd\">Delete</SetVar></Method>";
            BatchUtil.ProcessBatch(entities, web, ListName, ListItemFieldMapper, command, blocksize);
        }

        /// <summary>
        /// Массовое удаление сущностей, управляя срабатыванием приемников событий
        /// </summary>
        /// <param name="entities">Сущности</param>
        /// <param name="web">Веб-узел</param>
        /// <param name="eventFiringEnabled">true - если приемники событий должны срабатывать, false - в противном случае</param>
        /// <param name="blocksize">Количество удаляемых сущностей в рамках одного запроса к БД</param>
        /// <param name="batchFinishedFunc">Процедура, выполняемая после удаления блока элементов. Принимает количество удалённых элементов</param>
        public void DeleteBatch(TEntity[] entities, SPWeb web, bool eventFiringEnabled, int blocksize = 1000, Action<int> batchFinishedFunc = null)
        {
            if (!eventFiringEnabled)
            {
                using (new DisabledItemEventsScope())
                    DeleteBatch(entities, web);
            }
            else
                DeleteBatch(entities, web);
        }

        /// <summary>
        /// Массовое удаление сущностей по их идентификаторам
        /// </summary>
        /// <param name="entityIDs">Массив сущностей</param>
        /// <param name="web">Веб-узел</param>
        /// <param name="blocksize">Количество удаляемых сущностей в рамках одного запроса к БД</param>
        /// <param name="batchFinishedFunc">Процедура, выполняемая после удаления блока элементов. Принимает количество удалённых элементов</param>
        public void DeleteBatch(int[] entityIDs, SPWeb web, int blocksize = 1000, Action<int> batchFinishedFunc = null)
        {
            string command = "<Method><SetList Scope=\"Request\">{0}</SetList><SetVar Name=\"ID\">{1}</SetVar><SetVar Name=\"Cmd\">Delete</SetVar></Method>";
            SPList list = web.Lists[ListName];

            Func<int, string> methodExtractor = index => string.Format(command, list.ID, entityIDs[index]);
            BatchUtil.ProcessBatch(entityIDs.Length, methodExtractor, web, blocksize, batchFinishedFunc);
        }

        /// <summary>
        /// Массовое удаление сущностей по их идентификаторам, управляя срабатыванием приемников событий
        /// </summary>
        /// <param name="entityIDs">Массив сущностей</param>
        /// <param name="web">Веб-узел</param>
        /// <param name="eventFiringEnabled">true - если приемники событий должны срабатывать, false - в противном случае</param>
        /// <param name="blocksize">Количество удаляемых сущностей в рамках одного запроса к БД</param>
        /// <param name="batchFinishedFunc">Процедура, выполняемая после удаления блока элементов. Принимает количество удалённых элементов</param>
        public void DeleteBatch(int[] entityIDs, SPWeb web, bool eventFiringEnabled, int blocksize = 1000, Action<int> batchFinishedFunc = null)
        {
            if (!eventFiringEnabled)
            {
                using (new DisabledItemEventsScope())
                    DeleteBatch(entityIDs, web);
            }
            else
                DeleteBatch(entityIDs, web);
        }

        /// <summary>
        /// Удалить сущность
        /// </summary>
        /// <param name="entity">Удаляемая сущность</param>
        /// <param name="web">Узел, в котором необходимо удалить сущности</param>
        public void Delete(SPWeb web, TEntity entity)
        {
            Delete(web, entity.ID);
        }

        /// <summary>
        /// Удалить сущность, управляя срабатыванием приемников событий
        /// </summary>
        /// <param name="entity">Удаляемая сущность</param>
        /// <param name="web">Узел, в котором необходимо удалить сущность</param>
        /// <param name="eventFiringEnabled">true - если приемники событий должны срабатывать, false - в противном случае</param>
        public void Delete(SPWeb web, TEntity entity, bool eventFiringEnabled)
        {
            Delete(web, entity.ID, eventFiringEnabled);
        }

        /// <summary>
        ///  Удалить сущность по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор удаляемой сущности</param>
        /// <param name="web">Узел, в котором необходимо удалить сущности</param>
        public void Delete(SPWeb web, int id)
        {
            var listItemService = new ListItemService();
            listItemService.Delete(web.Lists[ListName], id);
        }

        /// <summary>
        /// Удалить сущность по идентификатору, управляя срабатыванием приемников событий
        /// </summary>
        /// <param name="id">Идентификатор удаляемой сущности</param>
        /// <param name="web">Узел, в котором необходимо удалить сущность</param>
        /// <param name="eventFiringEnabled">true - если приемники событий должны срабатывать, false - в противном случае</param>
        public void Delete(SPWeb web, int id, bool eventFiringEnabled)
        {
            if (!eventFiringEnabled)
            {
                using (new DisabledItemEventsScope())
                    Delete(web, id);
            }
            else
                Delete(web, id);
        }

        /// <summary>
        /// Удалить набор сущностей
        /// </summary>
        /// <param name="entities">Коллекция удаляемых сущностей</param>
        /// <param name="web">Узел, в котором необходимо удалить сущности</param>
        public void DeleteRange(SPWeb web, IEnumerable<TEntity> entities)
        {
            foreach (TEntity entity in entities)
                Delete(web, entity);
        }

        /// <summary>
        /// Удалить набор сущностей, управляя срабатыванием приемников событий
        /// </summary>
        /// <param name="entities">Коллекция удаляемых сущностей</param>
        /// <param name="web">Узел, в котором необходимо удалить сущности</param>
        /// <param name="eventFiringEnabled">true - если приемники событий должны срабатывать, false - в противном случае</param>
        public void DeleteRange(SPWeb web, IEnumerable<TEntity> entities, bool eventFiringEnabled)
        {
            if (!eventFiringEnabled)
            {
                using (new DisabledItemEventsScope())
                    DeleteRange(web, entities);
            }
            else
                DeleteRange(web, entities);
        }

        /// <summary>
        /// Удалить набор сущностей
        /// </summary>
        /// <param name="ids">Коллекция идентификаторов удаляемых сущностей</param>
        /// <param name="web">Узел, в котором необходимо удалить сущности</param>
        public void DeleteRange(SPWeb web, IEnumerable<int> ids)
        {
            foreach (int id in ids)
                Delete(web, id);
        }

        /// <summary>
        /// Удалить набор сущностей, управляя срабатыванием приемников событий
        /// </summary>
        /// <param name="ids">Коллекция идентификаторов удаляемых сущностей</param>
        /// <param name="web">Узел, в котором необходимо удалить сущности</param>
        /// <param name="eventFiringEnabled">true - если приемники событий должны срабатывать, false - в противном случае</param>
        public void DeleteRange(SPWeb web, IEnumerable<int> ids, bool eventFiringEnabled)
        {
            if (!eventFiringEnabled)
            {
                using (new DisabledItemEventsScope())
                    DeleteRange(web, ids);
            }
            else
                DeleteRange(web, ids);
        }

        public void DeleteFolder(SPWeb web, string folderListRelativeUrl)
        {
            using (new AllowUnsafeUpdates(web))
            {
                SPList list = web.Lists[this.ListName];
                FolderService folderService = new FolderService();
                string folderUrl = SPUtility.ConcatUrls(list.RootFolder.ServerRelativeUrl, folderListRelativeUrl);
                folderService.DeleteFolder(web, folderUrl);
            } 
        }

        #endregion

        #region Обновление элементов

        /// <summary>
        /// Обновление сущности
        /// </summary>
        /// <param name="entity">Сущность, нуждающаяся в обновлении</param>
        /// <param name="web">Узел, на котором необходимо обновить данные</param>
        private void UpdateAfterProperties(SPWeb web, TEntity entity, SPItemEventProperties properties)
        {
            try
            {
                ListItemFieldMapper.FillAfterPropertiesFromEntity(web, properties, typeof(TEntity), entity);
            }
            catch (Exception ex)
            {
                InvalidOperationException exception = new InvalidOperationException(string.Format("Ошибка при обновлении AfterProperties {0} узла {1}", ListName, web.Url), ex);
                throw exception;
            }
        }

        /// <summary>
        /// Обновить сущность
        /// </summary>
        /// <param name="entity">Сущность для обновления</param>
        /// <param name="web">Узел, на который необходимо добавить сущность</param>
        /// <param name="listItem">Элемент, который должен быть обновлен</param>
        /// <param name="trackChanges">Позволить ли SharePoint отслеживать изменения</param>
        /// <remarks>Необходимо для использования в EventReceiver'е и других местах,
        /// где данный элемент уже доступен</remarks>
        private void Update(SPWeb web, TEntity entity, SPListItem listItem, bool trackChanges)
        {
            if (entity.ID != listItem.ID)
                throw new ArgumentException("Сущность не соответствует элементу списка");

            UpdateListItemInternal(web, entity, listItem, trackChanges);
        }

        protected void UpdateListItemInternal(SPWeb web, TEntity entity, SPListItem listItem, bool trackChanges = true)
        {
            this.ListItemFieldMapper.FillSPListItemFromEntity(web, listItem, typeof(TEntity), entity);
            using (new AllowUnsafeUpdates(web))
            {
                if (trackChanges)
                    listItem.Update();
                else
                    listItem.SystemUpdate();
            }
        }

        /// <summary>
        /// Массовое обновление сущностей с учетом структуры папок
        /// </summary>
        /// <param name="entities">Сущности с указанием папок</param>
        /// <param name="web">Веб-узел</param>
        /// <param name="blocksize">Количество удаляемых сущностей в рамках одного запроса к БД</param>
        /// <param name="batchFinishedFunc">Процедура, выполняемая после обновления блока элементов. Принимает количество обновлённых элементов</param>
        public void UpdateBatch(TEntity[] entities, SPWeb web, int blocksize = 1000)
        {
            string command = "<Method ID=\"{0}\"><SetList>{1}</SetList><SetVar Name=\"ID\">{2}</SetVar><SetVar Name=\"Cmd\">Save</SetVar>{3}</Method>";
            BatchUtil.ProcessBatch(entities, web, ListName, ListItemFieldMapper, command, blocksize);
        }

        /// <summary>
        /// Массовое обновление сущностей, управляя срабатыванием приемников событий
        /// </summary>
        /// <param name="entities">Сущности с указанием папок</param>
        /// <param name="web">Веб-узел</param>
        /// <param name="eventFiringEnabled">true - если приемники событий должны срабатывать, false - в противном случае</param>
        /// <param name="blocksize">Количество удаляемых сущностей в рамках одного запроса к БД</param>
        /// <param name="batchFinishedFunc">Процедура, выполняемая после обновления блока элементов. Принимает количество обновлённых элементов</param>
        public void UpdateBatch(TEntity[] entities, SPWeb web, bool eventFiringEnabled, int blocksize = 1000)
        {
            if (!eventFiringEnabled)
            {
                using (new DisabledItemEventsScope())
                    UpdateBatch(entities, web, blocksize);
            }
            else
                UpdateBatch(entities, web, blocksize);
        }

        /// <summary>
        /// Обновить сущность, управляя срабатыванием приемников событий
        /// </summary>
        /// <param name="entity">Сущность для обновления</param>
        /// <param name="web">Узел, на который необходимо добавить сущность</param>
        /// <param name="trackChanges">Позволить ли SharePoint отслеживать изменения</param>
        /// <param name="eventFiringEnabled">true - если приемники событий должны срабатывать, false - в противном случае</param>
        public void Update(SPWeb web, TEntity entity, bool trackChanges, bool eventFiringEnabled)
        {
            if (!eventFiringEnabled)
            {
                using (new DisabledItemEventsScope())
                    Update(web, entity, trackChanges);
            }
            else
                Update(web, entity, trackChanges);
        }

        /// <summary>
        /// Обновить сущность
        /// </summary>
        /// <param name="entity">Сущность для обновления</param>
        /// <param name="web">Узел, на который необходимо добавить сущность</param>
        /// <param name="trackChanges">Позволить ли SharePoint отслеживать изменения</param>
        public virtual void Update(SPWeb web, TEntity entity)
        {
            try
            {
                if (entity.ListItem == null)
                    entity.ListItem = GetListItemById(web, entity.ID);
                UpdateListItemInternal(web, entity, entity.ListItem, true);
            }
            catch (Exception ex)
            {
                InvalidOperationException exception = new InvalidOperationException(string.Format("Ошибка при обновлении элемента списка {0} узла {1} с ID={2}", ListName, web.Url, entity.ID), ex);
                throw exception;
            }
        }

        /// <summary>
        /// Обновить сущность
        /// </summary>
        /// <param name="entity">Сущность для обновления</param>
        /// <param name="web">Узел, на который необходимо добавить сущность</param>
        /// <param name="trackChanges">Позволить ли SharePoint отслеживать изменения</param>
        public virtual void Update(SPWeb web, TEntity entity, bool trackChanges)
        {
            try
            {
                if (entity.ListItem == null)
                    entity.ListItem = web.Lists[this.ListName].GetItemById(entity.ID);

                UpdateListItemInternal(web, entity, entity.ListItem, trackChanges);
            }
            catch (Exception ex)
            {
                string message = $"Ошибка при обновлении элемента списка {ListName} узла {web.Url} с ID={entity.ID}";
                InvalidOperationException exception = new InvalidOperationException(message, ex);
                throw exception;
            }
        }

        /// <summary>
        /// Обновить несколько сущностей
        /// </summary>
        /// <param name="entities">Сущности для обновления</param>
        /// <param name="web">Узел, на который необходимо добавить сущности</param>
        public void UpdateRange(SPWeb web, IEnumerable<TEntity> entities, bool trackChanges)
        {
            foreach (TEntity entity in entities)
                Update(web, entity, trackChanges);
        }

        #endregion

        #region Ссылки на формы

        /// <summary>
        /// Получить веб-адрес представления по умолчанию списка
        /// </summary>
        /// <param name="web">Сайт</param>
        /// <returns>Веб-адрес представления по умолчанию списка</returns>
        public string GetDefaultViewUrl(SPWeb web)
        {
            SPList workList = this.GetList(web);
            return workList.DefaultViewUrl;
        }

        /// <summary>
        /// Получить абсолютный веб-адрес формы просмотра элементов списка
        /// </summary>
        /// <param name="web">Сайт</param>
        /// <returns>Абсолютный веб-адрес формы просмотра элементов списка</returns>
        public string GetFullDisplayFormUrl(SPWeb web)
        {
            SPList workList = this.GetList(web);
            string formUrl = workList.Forms[PAGETYPE.PAGE_DISPLAYFORM].Url;
            return SPUtility.ConcatUrls(web.Url, formUrl);
        }

        /// <summary>
        /// Получить абсолютный веб-адрес формы просмотра сущности
        /// </summary>
        /// <param name="web">Сайт</param>
        /// <returns>Абсолютный веб-адрес формы просмотра сущности</returns>
        public string GetFullDisplayFormUrl(SPWeb web, TEntity entity)
        {
            return GetFullEntityFormUrl(web, PAGETYPE.PAGE_DISPLAYFORM, entity);
        }

        /// <summary>
        /// Получить абсолютный веб-адрес формы просмотра сущности по её идентификатору
        /// </summary>
        /// <param name="web">Сайт</param>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Абсолютный веб-адрес формы просмотра сущности</returns>
        public string GetFullDisplayFormUrl(SPWeb web, int id)
        {
            return GetFullEntityFormUrl(web, PAGETYPE.PAGE_DISPLAYFORM, id);
        }

        /// <summary>
        /// Получить абсолютный веб-адрес формы создания сущности
        /// </summary>
        /// <param name="web">Сайт</param>
        /// <returns>Абсолютный веб-адрес формы создания сущности</returns>
        public string GetFullNewFormUrl(SPWeb web, TEntity entity)
        {
            return GetFullEntityFormUrl(web, PAGETYPE.PAGE_NEWFORM, entity);
        }

        /// <summary>
        /// Получить абсолютный веб-адрес формы создания сущности по её идентификатору
        /// </summary>
        /// <param name="web">Сайт</param>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Абсолютный веб-адрес формы создания сущности</returns>
        public string GetFullNewFormUrl(SPWeb web, int id)
        {
            return GetFullEntityFormUrl(web, PAGETYPE.PAGE_NEWFORM, id);
        }

        /// <summary>
        /// Получить абсолютный веб-адрес формы изменения сущности
        /// </summary>
        /// <param name="web">Сайт</param>
        /// <returns>Абсолютный веб-адрес формы изменения сущности</returns>
        public string GetFullEditFormUrl(SPWeb web, TEntity entity)
        {
            return GetFullEntityFormUrl(web, PAGETYPE.PAGE_EDITFORM, entity);
        }

        /// <summary>
        /// Получить абсолютный веб-адрес формы изменения сущности по её идентификатору
        /// </summary>
        /// <param name="web">Сайт</param>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Абсолютный веб-адрес формы изменения сущности</returns>
        public string GetFullEditFormUrl(SPWeb web, int id)
        {
            return GetFullEntityFormUrl(web, PAGETYPE.PAGE_EDITFORM, id);
        }

        /// <summary>
        /// Получить абсолютный Url-адрес формы для сущности
        /// </summary>
        /// <param name="web">Сайт</param>
        /// <param name="pagetype">Тип формы</param>
        /// <param name="entity">Сущность</param>
        /// <returns>Абсолютный веб-адрес формы сущности</returns>
        protected string GetFullEntityFormUrl(SPWeb web, PAGETYPE pagetype, TEntity entity)
        {
            return GetFullEntityFormUrl(web, pagetype, entity.ID);
        }

        /// <summary>
        /// Получить абсолютный Url-адрес формы для сущности по её идентификатору
        /// </summary>
        /// <param name="web">Сайт</param>
        /// <param name="pagetype">Тип формы</param>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Абсолютный веб-адрес формы сущности</returns>
        protected string GetFullEntityFormUrl(SPWeb web, PAGETYPE pagetype, int id)
        {
            SPList workList = this.GetList(web);
            string formUrl = string.Format("{0}?ID={1}", workList.Forms[pagetype].Url, id);
            return SPUtility.ConcatUrls(web.Url, formUrl);
        }

        /// <summary>
        /// Получить веб-адрес формы просмотра элементов списка (адрес относительно сервера)
        /// </summary>
        /// <param name="web">Сайт</param>
        /// <returns>Веб-адрес формы просмотра элементов списка</returns>
        public string GetDisplayFormUrl(SPWeb web)
        {
            SPList workList = this.GetList(web);
            return workList.Forms[PAGETYPE.PAGE_DISPLAYFORM].ServerRelativeUrl;
        }

        /// <summary>
        /// Получить веб-адрес формы просмотра сущности (адрес относительно сервера)
        /// </summary>
        /// <param name="web">Сайт</param>
        /// <returns>Веб-адрес формы просмотра сущности</returns>
        public string GetDisplayFormUrl(SPWeb web, TEntity entity)
        {
            return GetEntityFormUrl(web, PAGETYPE.PAGE_DISPLAYFORM, entity);
        }

        /// <summary>
        /// Получить веб-адрес формы просмотра сущности по её идентификатору (адрес относительно сервера)
        /// </summary>
        /// <param name="web">Сайт</param>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Веб-адрес формы просмотра сущности</returns>
        public string GetDisplayFormUrl(SPWeb web, int id)
        {
            return GetEntityFormUrl(web, PAGETYPE.PAGE_DISPLAYFORM, id);
        }

        /// <summary>
        /// Получить веб-адрес формы создания элементов списка (адрес относительно сервера)
        /// </summary>
        /// <param name="web">Сайт</param>
        /// <returns>Веб-адрес формы создания элементов списка</returns>
        public string GetNewFormUrl(SPWeb web)
        {
            SPList workList = this.GetList(web);
            return workList.Forms[PAGETYPE.PAGE_NEWFORM].ServerRelativeUrl;
        }

        /// <summary>
        /// Получить веб-адрес формы создания элементов списка с указанным ContentTypeId (адрес относительно сервера)
        /// </summary>
        /// <param name="web">Сайт</param>
        /// <returns>Веб-адрес формы создания элементов списка</returns>
        public string GetNewFormUrl(SPWeb web, SPContentTypeId contentTypeId)
        {
            SPList workList = this.GetList(web);
            string formUrl = workList.Forms[PAGETYPE.PAGE_NEWFORM].ServerRelativeUrl;
            return string.Format("{0}?ContentTypeId={1}", formUrl, contentTypeId);
        }

        /// <summary>
        /// Получить веб-адрес формы редактирования элементов списка (адрес относительно сервера)
        /// </summary>
        /// <param name="web">Сайт</param>
        /// <returns>Веб-адрес формы редактирования элементов списка</returns>
        public string GetEditFormUrl(SPWeb web)
        {
            SPList workList = this.GetList(web);
            return workList.Forms[PAGETYPE.PAGE_EDITFORM].ServerRelativeUrl;
        }

        /// <summary>
        /// Получить веб-адрес формы редактирования сущности (адрес относительно сервера)
        /// </summary>
        /// <param name="web">Сайт</param>
        /// <returns>Веб-адрес формы редактирования сущности</returns>
        public string GetEditFormUrl(SPWeb web, TEntity entity)
        {
            return GetEntityFormUrl(web, PAGETYPE.PAGE_EDITFORM, entity);
        }

        /// <summary>
        /// Получить веб-адрес формы редактирования сущности по её идентификатору (адрес относительно сервера)
        /// </summary>
        /// <param name="web">Сайт</param>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Веб-адрес формы редактирования сущности</returns>
        public string GetEditFormUrl(SPWeb web, int id)
        {
            return GetEntityFormUrl(web, PAGETYPE.PAGE_EDITFORM, id);
        }

        /// <summary>
        /// Получить Url-адрес формы для сущности (адрес относительно сервера)
        /// </summary>
        /// <param name="web">Сайт</param>
        /// <param name="pagetype">Тип формы</param>
        /// <param name="entity">Сущность</param>
        /// <returns>Веб-адрес формы сущности</returns>
        protected string GetEntityFormUrl(SPWeb web, PAGETYPE pagetype, TEntity entity)
        {
            return GetEntityFormUrl(web, pagetype, entity.ID);
        }

        /// <summary>
        /// Получить Url-адрес формы для сущности по её идентификатору (адрес относительно сервера)
        /// </summary>
        /// <param name="web">Сайт</param>
        /// <param name="pagetype">Тип формы</param>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Веб-адрес формы сущности</returns>
        protected string GetEntityFormUrl(SPWeb web, PAGETYPE pagetype, int id)
        {
            SPList workList = this.GetList(web);
            string formUrl = workList.Forms[pagetype].ServerRelativeUrl;
            return $"{formUrl}?ID={id}";
        }

        #endregion

        #region Загрузить файл(ы)

        /// <summary>
        /// Загрузить файл в библиотеку документов с указанием папки для загрузки
        /// </summary>
        /// <param name="web">Веб-сайт библиотеки</param>
        /// <param name="folderRelativeUrl">Относительный адрес папки</param>
        /// <param name="fileName">Наименование файла</param>
        /// <param name="content">Содержимое файла</param>
        /// <param name="replaceExistingFiles">Заменять существующие файлы</param>
        /// <returns>Загруженный в библиотеку документов файл</returns>
        public SPFile UploadFile(SPWeb web, string folderRelativeUrl, string fileName, byte[] content, bool replaceExistingFiles)
        {
            SPFolder targetFolder = CreateFolder(web, folderRelativeUrl);
            return UploadFile(web, targetFolder, fileName, content, replaceExistingFiles);
        }

        /// <summary>
        /// Загрузить файл в библиотеку документов с указанием папки для загрузки
        /// </summary>
        /// <param name="web">Веб-сайт библиотеки</param>
        /// <param name="folderRelativeUrl">Относительный адрес папки</param>
        /// <param name="fileName">Наименование файла</param>
        /// <param name="stream">Поток байтов файла</param>
        /// <param name="replaceExistingFiles">Заменять существующие файлы</param>
        /// <returns>Загруженный в библиотеку документов файл</returns>
        public SPFile UploadFile(SPWeb web, string folderRelativeUrl, string fileName, Stream stream, bool replaceExistingFiles)
        {
            SPFolder targetFolder = CreateFolder(web, folderRelativeUrl);
            return UploadFile(web, targetFolder, fileName, stream, replaceExistingFiles);
        }

        /// <summary>
        /// Загрузить файл в библиотеку документов
        /// </summary>
        /// <param name="web">Веб-сайт библиотеки</param>
        /// <param name="fileName">Наименование файла</param>
        /// <param name="content">Содержимое файла</param>
        /// <param name="replaceExistingFiles">Заменять существующие файлы</param>
        /// <returns>Загруженный в библиотеку документов файл</returns>
        public SPFile UploadFile(SPWeb web, string fileName, byte[] content, bool replaceExistingFiles)
        {
            SPList list = GetList(web);
            return UploadFile(web, list.RootFolder, fileName, content, replaceExistingFiles);
        }

        /// <summary>
        /// Загрузить файл в библиотеку документов
        /// </summary>
        /// <param name="web">Веб-сайт библиотеки</param>
        /// <param name="fileName">Наименование файла</param>
        /// <param name="stream">Поток байтов файла</param>
        /// <param name="replaceExistingFiles">Заменять существующие файлы</param>
        /// <returns>Загруженный в библиотеку документов файл</returns>
        public SPFile UploadFile(SPWeb web, string fileName, Stream stream, bool replaceExistingFiles)
        {
            SPList list = GetList(web);
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);

            return UploadFile(web, list.RootFolder, fileName, buffer, replaceExistingFiles);
        }

        /// <summary>
        /// Загрузить файл в папку
        /// </summary>
        /// <param name="web">Веб-сайт</param>
        /// <param name="targetFolder">Целевая папка</param>
        /// <param name="fileName">Наименование файла</param>
        /// <param name="content">Содержимое файла</param>
        /// <param name="replaceExistingFiles">Заменять существующие файлы</param>
        /// <returns>Загруженный в библиотеку документов файл</returns>
        public SPFile UploadFile(SPWeb web, SPFolder targetFolder, string fileName, byte[] content, bool replaceExistingFiles)
        {
            using (new AllowUnsafeUpdates(web))
            {
                return targetFolder
                    .Files
                    .Add(fileName, content, replaceExistingFiles);
            }
        }

        /// <summary>
        /// Загрузить файл в папку
        /// </summary>
        /// <param name="web">Веб-сайт</param>
        /// <param name="targetFolder">Целевая папка</param>
        /// <param name="fileName">Наименование файла</param>
        /// <param name="stream">Поток байтов файла</param>
        /// <param name="replaceExistingFiles">Заменять существующие файлы</param>
        /// <returns>Загруженный в библиотеку документов файл</returns>
        public SPFile UploadFile(SPWeb web, SPFolder targetFolder, string fileName, Stream stream, bool replaceExistingFiles)
        {
            bool oldAllowUnsafeUpdates = web.AllowUnsafeUpdates;
            web.AllowUnsafeUpdates = true;
            SPFile file = targetFolder.Files.Add(fileName, stream, replaceExistingFiles);
            web.AllowUnsafeUpdates = oldAllowUnsafeUpdates;
            return file;
        }

        #endregion

        #region Work With Attachments

        /// <summary>
        /// Возвращает вложения к сущности
        /// </summary>
        /// <param name="web">Веб-сайт</param>
        /// <param name="entity">Сущность</param>
        /// <returns>Вложения</returns>
        public SPFile[] GetAttachments(SPWeb web, TEntity entity)
        {
            var attachementsFolder = web.GetFolder(entity.AttachmentUrlPrefix);
            return attachementsFolder.Files.Cast<SPFile>().ToArray();
        }

        /// <summary>
        /// Загрузить вложение к сущности
        /// </summary>
        /// <param name="web">Веб-сайт</param>
        /// <param name="entity">Сущность</param>
        /// <param name="content">Бинарное содержимое файла вложения</param>
        /// <returns>Загруженное вложение</returns>
        public string UploadAttachment(SPWeb web, TEntity entity, string name, byte[] content)
        {
            using (new AllowUnsafeUpdates(web))
            {
                return entity.ListItem.Attachments.AddNow(name, content);
            }
        }

        /// <summary>
        /// Загрузить вложение к сущности
        /// </summary>
        /// <param name="web">Веб-сайт</param>
        /// <param name="entity">Сущность</param>
        /// <param name="stream">Поток байтов файла вложения</param>
        /// <returns>Загруженное вложение</returns>
        public string UploadAttachment(SPWeb web, TEntity entity, string name, Stream stream)
        {
            byte[] content = new byte[stream.Length];
            stream.Read(content, 0, (int)stream.Length);

            using (new AllowUnsafeUpdates(web))
                return entity.ListItem.Attachments.AddNow(name, content);
        }

        /// <summary>
        /// Удалить вложение сущности
        /// </summary>
        /// <param name="web">Веб-сайт</param>
        /// <param name="entity">Сущность</param>
        /// <param name="name">Название вложения</param>
        public void DeleteAttachment(SPWeb web, TEntity entity, string name)
        {
            using (new AllowUnsafeUpdates(web))
                entity.ListItem.Attachments.DeleteNow(name);
        }

        #endregion

        #region Lock/Unlock Files

        /// <summary>
        /// Разблокирует файл библиотеки (не зависимо от учетной записи), связанный с сущностью
        /// </summary>
        /// <param name="web">Веб-сайт</param>
        /// <param name="entity">Сущность</param>
        public void Unlock(SPWeb web, TEntity entity)
        {
            SPListItem item = entity.ListItem;
            SPUser lockuser = item.File.LockedByUser;

            using (SPSite newSite = new SPSite(web.Site.ID, lockuser.UserToken))
            using (SPWeb newWeb = newSite.OpenWeb())
            {
                TEntity newEntity = GetEntityById(web, entity.ID);
                ReleaseLock(newWeb, newEntity);
            }
        }

        /// <summary>
        /// Разблокирует файл библиотеки, связанный с сущностью и заблокированный ранее текущим пользователем
        /// </summary>
        /// <param name="web">Веб-сайт</param>
        /// <param name="entity">Сущность</param>
        public void ReleaseLock(SPWeb web, TEntity entity)
        {
            SPFile documentFile = entity.File;
            documentFile.ReleaseLock(documentFile.LockId);
        }

        #endregion

        #region Other Methods

        /// <summary>
        /// Получить список. Если список на узле не найден, то возвращается null.
        /// </summary>
        /// <param name="web">Узел, на котором находится список</param>
        /// <param name="listName">Имя списка</param>
        /// <returns>Необходимый список</returns>
        public SPList GetList(SPWeb web)
        {
            return web.Lists.TryGetList(ListName);
        }

        /// <summary>
        /// Проверка на существование списка
        /// </summary>
        /// <param name="web"></param>
        /// <returns></returns>
        public bool IsListExists(SPWeb web)
        {
            return GetList(web) != null;
        }

        /// <summary>
        /// Получение поля по имени свойства
        /// </summary>
        /// <param name="propertyName">Название свойства сущности</param>
        /// <returns>Поле списка, соответствующее свойству сущности</returns>
        public SPField GetField(SPWeb web, string propertyName)
        {
            try
            {
                string fieldName = ListItemFieldMapper.GetFieldDisplayName(propertyName);
                SPList list = GetList(web);
                return list.Fields.GetField(fieldName);
            }
            catch (Exception ex)
            {
                InvalidOperationException exception = new InvalidOperationException(string.Format("Ошибка при получении поля, соответствующего свойству {0}", propertyName), ex);
                throw exception;
            }
        }

        private SPFolder CreateFolder(SPWeb web, string folderRelativeUrl)
        {
            SPList list = GetList(web);
            SPFolder targetFolder = list.RootFolder;
            if (!string.IsNullOrEmpty(folderRelativeUrl))
            {
                FolderService folderService = new FolderService();
                folderService.CreatePath(web, list, folderRelativeUrl);
                targetFolder = folderService.EnsureFolder(web, list.RootFolder.Url, folderRelativeUrl);
            }
            return targetFolder;
        }

        /// <summary>
        /// Проверить существует ли сущность с указанным идентификатором на веб-сайте
        /// </summary>
        /// <param name="web">Веб-сайт</param>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>True - если сущность существует на указанном веб-сайте, в противном случае - false</returns>
        public bool EntityExists(SPWeb web, int id)
        {
            SPListItem item = GetListItemById(web, id);
            return item != null;
        }

        /// <summary>
        /// Проверка существования необходимых полей
        /// </summary>
        /// <param name="item">Элемент, существование полей в котором необходимо проверить</param>
        public void CheckExistsFields(SPListItem item)
        {
            List<string> missingFields = new List<string>();

            foreach (var mapping in ListItemFieldMapper.Mappings)
            {
                if (!item.Fields.ContainsField(mapping.FieldName))
                    missingFields.Add(mapping.FieldName);
            }

            if (missingFields.Count > 0)
            {
                ArgumentException ex = new ArgumentException(
                    string.Format(
                    CultureInfo.CurrentCulture,
                    "Следующие поля отсутствуют: {0}",
                    string.Join(", ", missingFields.ToArray())));
                throw ex;
            }
        }

        public bool FolderExists(SPWeb web, string url)
        {
            try
            {
                if (web.GetFolder(url).Exists)
                {
                    return true;
                }
                return false;
            }
            catch (ArgumentException) { return false; }
            catch (Exception) { return false; }
        }

        #endregion
    }
}