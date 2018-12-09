using Microsoft.SharePoint;
using SP2016.Repository.Converters;
using SP2016.Repository.Converters.SharePoint;
using SP2016.Repository.Entities;
using SP2016.Repository.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SP2016.Repository.Mapping.SharePoint
{
    public class SPAfterPropertiesMapper<TEntity> : SPFieldMapper<TEntity> where TEntity : BaseEntity
    {
        public SPAfterPropertiesMapper(IEnumerable<FieldToPropertyMapping> mappings)
            : base(mappings)
        {
            var afterPropertiesMapper = new (Type, IConverter)[] {
                (typeof(DateTime), new XmlDateTimeFieldValueConverter()),
                (typeof(SPContentTypeId), new SPContentTypeIdValueConverter()),
                (typeof(SPFieldUserValueCollection), new SPFieldUserValueCollectionConverter())
            };

            RegisterUniqueConverters(afterPropertiesMapper);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="web"></param>
        /// <param name="entity">Map to entity</param>
        /// <param name="properties">Map from properties</param>
        public void Map(SPWeb web, BaseEntity entity, SPItemEventProperties properties)
        {
            SPList list = properties.List;

            foreach (FieldToPropertyMapping fieldMapping in FieldMappings)
            {
                PropertyInfo propertyInfo = ReflectionUtil.GetPropertyInfo(typeof(TEntity), fieldMapping);
                SPField field = list.Fields.GetField(fieldMapping.FieldName);
                object fieldValue;

                if (properties.AfterProperties.Cast<DictionaryEntry>().Any(e => e.Key.Equals(field.InternalName)))
                {
                    fieldValue = properties.AfterProperties[field.InternalName];
                }
                else
                {
                    fieldValue = properties.ListItem ?? properties.ListItem[fieldMapping.FieldName];
                }

                var propertyValue = ConvertFieldValueToPropertyValue(web, field, propertyInfo, fieldValue);
                propertyInfo.SetValue(entity, propertyValue);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="web"></param>
        /// <param name="properties">Map to properties</param>
        /// <param name="entity">Map from entity</param>
        public void Map(SPWeb web, SPItemEventProperties properties, BaseEntity entity)
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
