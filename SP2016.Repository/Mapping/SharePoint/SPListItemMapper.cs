using Microsoft.SharePoint;
using SP2016.Repository.Entities;
using System;
using System.Collections.Generic;

namespace SP2016.Repository.Mapping.SharePoint
{
    public class SPListItemMapper<TEntity> : SPFieldMapper<TEntity> where TEntity : BaseSPEntity
    {
        public SPListItemMapper(IEnumerable<FieldToPropertyMapping> mappings) 
            : base(mappings) { }

        public void Map(SPWeb web, SPListItem to, BaseSPEntity from)
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

            foreach (var fieldMappingToPropertyValue in fieldMappingsToPropertyValues)
            {
                (FieldToPropertyMapping fieldMapping, object propertyValue) = fieldMappingToPropertyValue;
                fieldMapping.PropertyInfo.SetValue(to, propertyValue);
            }
        }
    }
}
