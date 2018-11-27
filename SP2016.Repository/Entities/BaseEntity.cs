using Microsoft.SharePoint;
using System;
using System.Runtime.Serialization;

namespace SP2016.Repository.Entities
{
    /// <summary>
    /// Базовый класс для всех сущностей
    /// </summary>
    public class BaseEntity : IEntity
    {
        /// <summary>
        /// Идентификатор элемента
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Идентификатор сущности
        /// </summary>
        public Guid GUID { get; set; }

        /// <summary>
        /// Кем создано
        /// </summary>
        public SPFieldUserValue Author { get; set; }

        /// <summary>
        /// Кем изменено
        /// </summary>
        public SPFieldUserValue Editor { get; set; }

        /// <summary>
        /// Дата изменения
        /// </summary>
        public DateTime Modified { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Имя файла библиотеки документов, связанной с текущей сущностью
        /// </summary>
        public string FileLeafRef { get; set; }

        /// <summary>
        /// Элемент списка. Использовать при крайней необходимости.
        /// </summary>
        [IgnoreDataMember]
        public SPListItem ListItem { get; set; }

        /// <summary>
        /// Тип контента для установки
        /// </summary>
        [IgnoreDataMember]
        private SPContentType SPContentType { get; set; }

        #region readonly fields

        /// <summary>
        /// Тип содержимого
        /// </summary>
        [IgnoreDataMember]
        public SPContentType ContentType
        {
            get
            {
                if (ListItem != null)
                    return ListItem.ContentType;
                else
                    return SPContentType;
            }
            set
            {
                SPContentType = value;
            }
        }

        /// <summary>
        /// Идентификатор типа содержимого
        /// </summary>
        public SPContentTypeId? ContentTypeId
        {
            get
            {
                if (ListItem != null)
                    return ListItem.ContentTypeId;
                return null;
            }
        }

        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        public Guid UniqueId
        {
            get
            {
                if (ListItem != null)
                    return ListItem.UniqueId;
                else
                    return new Guid();
            }
        }

        /// <summary>
        /// Преобразовать в подстановочное значение
        /// </summary>
        /// <returns>Подстановочное значение</returns>
        public SPFieldLookupValue ToLookup()
        {
            return new SPFieldLookupValue(ID, string.Empty);
        }

        /// <summary>
        /// Возвращает полный путь к папке вложений элемента в формате http://&lt;ServerDNSName&gt;:Port/.../ListName/Attachments/1/
        /// </summary>
        public string AttachmentUrlPrefix
        {
            get
            {
                if (ListItem == null) return null;
                else return ListItem.Attachments.UrlPrefix;
            }
        }

        /// <summary>
        /// Файл библиотеки документов, связанный с текущей сущностью
        /// </summary>
        public SPFile File
        {
            get
            {
                if (ListItem == null) return null;
                else return ListItem.File;
            }
        }

        #endregion
    }
}
