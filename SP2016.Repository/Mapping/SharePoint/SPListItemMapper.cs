using Microsoft.SharePoint;
using SP2016.Repository.Entities;
using SP2016.Repository.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SP2016.Repository.Mapping.SharePoint
{
    public class SPListItemMapper<TEntity> : SPFieldMapper<TEntity> where TEntity : BaseEntity
    {
        public SPListItemMapper(IEnumerable<FieldToPropertyMapping> mappings) 
            : base(mappings) { }

        public void Map(SPWeb web, SPListItem to, BaseEntity from)
        {
            var fieldMappingsToFieldValues = FieldMappingsToFieldValues(web, to.ParentList, from);

            foreach (var fieldMappingsToFieldValue in fieldMappingsToFieldValues)
            {
                (FieldToPropertyMapping fieldMapping, object fieldValue) = fieldMappingsToFieldValue;

                try
                {
                    to[fieldMapping.FieldName] = fieldValue;
                }
                catch (Exception ex)
                {
                    string errorMessage = $@"Не удалось записать значение {fieldValue} в поле {fieldMapping.FieldName} 
                                для сущности {typeof(TEntity).FullName}.";
                    var nextException = new Exception(errorMessage, ex);
                    Logger.Error(errorMessage, nextException);
                    throw nextException;
                }
            }
        }

        public void Map(SPWeb web, object to, SPListItem from)
        {
            var fieldMappingsToPropertyValues = FieldMappingsToPropertyValues(web, from.ParentList, from);

            foreach (var fieldMappingsToPropertyValue in fieldMappingsToPropertyValues)
            {
                (FieldToPropertyMapping fieldMapping, object propertyValue) = fieldMappingsToPropertyValue;

                PropertyInfo propertyInfo = ReflectionUtil.GetPropertyInfo(from, typeof(TEntity), fieldMapping);
                propertyInfo.SetValue(to, propertyValue);
            }
        }
    }
}
