using Microsoft.SharePoint;
using SP2016.Repository.Mapping;
using System;

namespace SP2016.Repository
{
    public static class ItemFieldsChecking
    {
        /// <summary>
        /// Проверка существования поля у элемента списка
        /// </summary>
        /// <param name="item">Элемент списка</param>
        /// <param name="entityType">Тип сущности</param>
        /// <param name="fieldMapping">Сопоставление, соответствующее проверяемому полю</param>
        /// <returns>Поле, которое необходимо было проверить</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", Justification = "Используется для проверки сущствования поля", MessageId = "ensuredField")]
        public static SPField EnsureListFieldID(SPListItem item, Type entityType, FieldToPropertyMapping fieldMapping)
        {
            try
            {
                return item.Fields.GetField(fieldMapping.FieldName);
            }
            catch (Exception ex)
            {
                string errorMessage = $@"SPListItem '{item.Name}' does not have a field with Id '{fieldMapping.FieldName}' 
                        which was mapped to property: '{fieldMapping.EntityPropertyName}' for entity '{entityType.FullName}'.";
                throw new Exception(errorMessage, ex);
            }
        }

        /// <summary>
        /// Проверка существования поля в списке
        /// </summary>
        /// <param name="list">Список</param>
        /// <param name="entityType">Тип сущности</param>
        /// <param name="fieldMapping">Сопоставление, соответствующее проверяемому полю</param>
        /// <returns>Поле, которое необходимо было проверить</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", Justification = "Используется для проверки сущствования поля", MessageId = "ensuredField")]
        public static SPField EnsureListFieldID(SPList list, Type entityType, FieldToPropertyMapping fieldMapping)
        {
            try
            {
                return list.Fields.GetField(fieldMapping.FieldName);
            }
            catch (Exception ex)
            {
                string errorMessage = $@"SPList '{list.Title}' does not have a field with Id '{fieldMapping.FieldName}'
                    which was mapped to property: '{fieldMapping.EntityPropertyName}' for entity '{entityType.FullName}'.";
                throw new Exception(errorMessage, ex);
            }
        }

        /// <summary>
        /// Проверка существования поля у элемента списка
        /// </summary>
        /// <param name="item">Элемент списка</param>
        /// <param name="entityType">Тип сущности</param>
        /// <param name="fieldMapping">Сопоставление, соответствующее проверяемому полю</param>
        /// <returns>Поле, которое необходимо было проверить</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", Justification = "Используется для проверки сущствования поля", MessageId = "ensuredField")]
        public static SPField EnsureListFieldID(SPListItemVersion item, Type entityType, FieldToPropertyMapping fieldMapping)
        {
            try
            {
                return item.Fields.GetField(fieldMapping.FieldName);
            }
            catch (ArgumentException ex)
            {
                string errorMessage = $@"SPListItem '{item.ListItem.Name}' does not have a field with Id '{fieldMapping.FieldName}' 
                            which was mapped to property: '{fieldMapping.EntityPropertyName}' for entity '{entityType.FullName}'.";
                throw new Exception(errorMessage, ex);
            }
        }

    }
}
