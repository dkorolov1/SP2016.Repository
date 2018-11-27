using Microsoft.SharePoint;
using SP2016.Repository.Mapping;
using System;
using System.Globalization;
using System.Reflection;

namespace SP2016.Repository.Utils
{
    public static class ReflectionUtil
    {
        /// <summary>
        /// Получение информации о свойстве сущности
        /// </summary>
        /// <param name="entityType">Тип сущности</param>
        /// <param name="fieldMapping">Сопоставление, соответствующее проверяемому полю</param>
        /// <returns>Информацию о соответствующем свойстве</returns>
        public static PropertyInfo GetPropertyInfo(Type entityType, FieldToEntityPropertyMapping fieldMapping)
        {
            PropertyInfo propertyInfo = entityType.GetProperty(fieldMapping.EntityPropertyName);
            if (propertyInfo == null)
            {
                string errorMessage = string.Format(
                    CultureInfo.CurrentCulture,
                    "Type '{0}' does not have a property '{1}' which was mapped to FieldID: '{2}'.",
                    entityType.FullName,
                    fieldMapping.EntityPropertyName,
                    fieldMapping.FieldName);
                throw new ListItemFieldMappingException(errorMessage);
            }

            return propertyInfo;
        }

        /// <summary>
        /// Получение информации о свойстве сущности
        /// </summary>
        /// <param name="entityType">Тип сущности</param>
        /// <param name="fieldMapping">Сопоставление, соответствующее проверяемому полю</param>
        /// <returns>Информацию о соответствующем свойстве</returns>
        public static PropertyInfo GetPropertyInfo(Type entityType, string propertyName)
        {
            PropertyInfo propertyInfo = entityType.GetProperty(propertyName);
            if (propertyInfo == null)
            {
                string errorMessage = string.Format(CultureInfo.CurrentCulture, "Type '{0}' does not have a property '{1}'.", entityType.FullName, propertyName);
                throw new ListItemFieldMappingException(errorMessage);
            }

            return propertyInfo;
        }

        /// <summary>
        /// Получение информации о свойстве сущности
        /// </summary>
        /// <param name="item">Элемент списка</param>
        /// <param name="entityType">Тип сущности</param>
        /// <param name="fieldMapping">Сопоставление, соответствующее проверяемому полю</param>
        /// <returns>Информацию о соответствующем свойстве</returns>
        public static PropertyInfo GetPropertyInfo(SPListItem item, Type entityType, FieldToEntityPropertyMapping fieldMapping)
        {
            PropertyInfo propertyInfo = entityType.GetProperty(fieldMapping.EntityPropertyName);
            if (propertyInfo == null)
            {
                string errorMessage = string.Format(
                    CultureInfo.CurrentCulture,
                    "Type '{0}' does not have a property '{1}' which was mapped to FieldID: '{2}' for SPListItem '{3}'.",
                    entityType.FullName,
                    fieldMapping.EntityPropertyName,
                    fieldMapping.FieldName,
                    item.Name);
                throw new ListItemFieldMappingException(errorMessage);
            }

            return propertyInfo;
        }

        /// <summary>
        /// Получение информации о свойстве сущности
        /// </summary>
        /// <param name="item">Элемент списка</param>
        /// <param name="entityType">Тип сущности</param>
        /// <param name="fieldMapping">Сопоставление, соответствующее проверяемому полю</param>
        /// <returns>Информацию о соответствующем свойстве</returns>
        public static PropertyInfo GetPropertyInfo(SPListItemVersion item, Type entityType, FieldToEntityPropertyMapping fieldMapping)
        {
            PropertyInfo propertyInfo = entityType.GetProperty(fieldMapping.EntityPropertyName);
            if (propertyInfo == null)
            {
                string errorMessage = string.Format(
                    CultureInfo.CurrentCulture,
                    "Type '{0}' does not have a property '{1}' which was mapped to FieldID: '{2}' for SPListItem '{3}'.",
                    entityType.FullName,
                    fieldMapping.EntityPropertyName,
                    fieldMapping.FieldName,
                    item.ListItem.Name);
                throw new ListItemFieldMappingException(errorMessage);
            }

            return propertyInfo;
        }
    }
}

