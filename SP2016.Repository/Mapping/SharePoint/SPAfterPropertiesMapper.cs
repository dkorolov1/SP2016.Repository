using Microsoft.SharePoint;
using SP2016.Repository.Converters.SharePoint;
using SP2016.Repository.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SP2016.Repository.Mapping.SharePoint
{
    public class SPAfterPropertiesMapper<TEntity> : SPFieldMapper<TEntity> where TEntity : BaseSPEntity
    {
        public SPAfterPropertiesMapper(IEnumerable<FieldToPropertyMapping> mappings)
            : base(mappings)
        {
            RegisterConverters(
                (typeof(DateTime), new XmlDateTimeFieldValueConverter()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="web"></param>
        /// <param name="entity">Map to entity</param>
        /// <param name="properties">Map from properties</param>
        public void Map(SPWeb web, BaseSPEntity entity, SPItemEventProperties properties)
        {
            SPList list = properties.List;

            foreach (FieldToPropertyMapping fieldMapping in FieldMappings)
            {
                SPField field = list.Fields.GetField(fieldMapping.FieldName);
                object fieldValue;
                bool hasProperty = properties.AfterProperties
                    .Cast<DictionaryEntry>()
                    .Any(e => e.Key.Equals(field.InternalName));

                if (hasProperty)
                {
                    fieldValue = properties
                        .AfterProperties[field.InternalName];
                }
                else
                {
                    fieldValue = properties.ListItem 
                        ?? properties.ListItem[fieldMapping.FieldName];
                }
                
                var propertyValue = ToPropertyValue(web, field, fieldMapping, fieldValue);

                fieldMapping.PropertyInfo.SetValue(entity, propertyValue);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="web"></param>
        /// <param name="properties">Map to properties</param>
        /// <param name="entity">Map from entity</param>
        public void Map(SPWeb web, SPItemEventProperties properties, BaseSPEntity entity)
        {
            var fieldMappingsToFieldValues = FieldMappingsToFieldValues(web, properties.List, entity);

            foreach (var fieldMappingsToFieldValue in fieldMappingsToFieldValues)
            {
                (FieldToPropertyMapping fieldMapping, object fieldValue) = fieldMappingsToFieldValue;

                try
                {
                    properties.AfterProperties[fieldMapping.FieldName] = fieldValue;
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
    }
}
