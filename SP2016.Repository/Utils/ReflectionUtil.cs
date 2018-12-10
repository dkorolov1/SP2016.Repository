using Microsoft.SharePoint;
using SP2016.Repository.Mapping;
using System;
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
        public static PropertyInfo GetPropertyInfo(Type entityType, FieldToPropertyMapping fieldMapping)
        {
            PropertyInfo propertyInfo = entityType.GetProperty(fieldMapping.EntityPropertyName);
            if (propertyInfo == null)
            {
                string errorMessage = $"Type '{entityType.FullName}' does not have a property '{fieldMapping.EntityPropertyName}' " +
                    $"which was mapped to FieldID: '{fieldMapping.FieldName}'.";
                throw new Exception(errorMessage);
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
                string errorMessage = $"Type '{entityType.FullName}' does not have a property '{propertyName}'.";
                throw new Exception(errorMessage);
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
        public static PropertyInfo GetPropertyInfo(SPListItem item, Type entityType, FieldToPropertyMapping fieldMapping)
        {
            PropertyInfo propertyInfo = entityType.GetProperty(fieldMapping.EntityPropertyName);
            if (propertyInfo == null)
            {
                string errorMessage = $@"Type '{entityType.FullName}' does not have a property '{fieldMapping.EntityPropertyName}' 
                        which was mapped to FieldID: '{fieldMapping.FieldName}' for SPListItem '{item.Name}'.";
                throw new Exception(errorMessage);
            }

            return propertyInfo;
        }
    }
}

