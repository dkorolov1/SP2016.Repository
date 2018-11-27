using Microsoft.SharePoint;
using SP2016.Repository.Mapping;
using System;
using System.Globalization;

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
        public static SPField EnsureListFieldID(SPListItem item, Type entityType, FieldToEntityPropertyMapping fieldMapping)
        {
            try
            {
                return item.Fields.GetField(fieldMapping.FieldName);
            }
            catch (ArgumentException argumentException)
            {
                string errorMessage = string.Format(
                    CultureInfo.CurrentCulture,
                    "SPListItem '{0}' does not have a field with Id '{1}' which was mapped to property: '{2}' for entity '{3}'.",
                    item.Name,
                    fieldMapping.FieldName,
                    fieldMapping.EntityPropertyName,
                    entityType.FullName);

                throw new ListItemFieldMappingException(errorMessage, argumentException);
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
        public static SPField EnsureListFieldID(SPList list, Type entityType, FieldToEntityPropertyMapping fieldMapping)
        {
            try
            {
                return list.Fields.GetField(fieldMapping.FieldName);
            }
            catch (ArgumentException argumentException)
            {
                string errorMessage = string.Format(
                    CultureInfo.CurrentCulture,
                    "SPList '{0}' does not have a field with Id '{1}' which was mapped to property: '{2}' for entity '{3}'.",
                    list.Title,
                    fieldMapping.FieldName,
                    fieldMapping.EntityPropertyName,
                    entityType.FullName);

                throw new ListItemFieldMappingException(errorMessage, argumentException);
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
        public static SPField EnsureListFieldID(SPListItemVersion item, Type entityType, FieldToEntityPropertyMapping fieldMapping)
        {
            try
            {
                return item.Fields.GetField(fieldMapping.FieldName);
            }
            catch (ArgumentException argumentException)
            {
                string errorMessage = string.Format(
                    CultureInfo.CurrentCulture,
                    "SPListItem '{0}' does not have a field with Id '{1}' which was mapped to property: '{2}' for entity '{3}'.",
                    item.ListItem.Name,
                    fieldMapping.FieldName,
                    fieldMapping.EntityPropertyName,
                    entityType.FullName);

                throw new ListItemFieldMappingException(errorMessage, argumentException);
            }
        }

    }
}
