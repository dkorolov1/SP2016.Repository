using Microsoft.SharePoint;
using SP2016.Repository.Converters;
using SP2016.Repository.Converters.SharePoint;
using SP2016.Repository.Entities;
using SP2016.Repository.Models;
using System;
using System.Collections.Generic;

namespace SP2016.Repository.Mapping.SharePoint
{
    public abstract class SPFieldMapper<TEntity> : FieldMapper where TEntity : BaseSPEntity
    {
        public SPFieldMapper(IEnumerable<FieldToPropertyMapping> mappings) 
            : base(mappings)
        {
            var spConverters = new (Type, FieldConverter)[] {
                (typeof(SPPrincipal), new SPPrincipalConverter()),
                (typeof(SPUser), new SPPrincipalConverter()),
                (typeof(SPGroup), new SPPrincipalConverter()),
                (typeof(SPPrincipalInfo), new SPPrincipalInfoConverter()),
                (typeof(SPFieldLookupValue), new SPFieldLookupConverter()),
                (typeof(SPFieldLookupValueCollection), new SPFieldLookupConverter()),
                (typeof(SPFieldMultiChoiceValue), new SPFieldMultiChoiceValueConverter()),
                (typeof(SPFieldUrlValue), new SPFieldUrlValueConverter()),
                (typeof(SPFieldUserValue), new SPFieldUserConverter()),
                (typeof(SPFieldUserValueCollection) , new SPFieldUserConverter()),
                (typeof(CalendarReccurenceData), new CalendarReccurenceDataConverter()),
                (typeof(SPFieldCalculatedValue), new SPFieldCalculatedValueConverter()),
                (typeof(SPContentType), new SPContentTypeValueConverter()),
                (typeof(SPContentTypeId), new SPContentTypeIdValueConverter()),
                (typeof(SPFieldUserValueCollection), new SPFieldUserValueCollectionConverter())
            };

            RegisterConverters(spConverters);
        }

        protected virtual FieldConverter GetConverter(SPWeb web, SPField field, FieldToPropertyMapping mapping)
        {
            FieldConverter converter = GetConverter(mapping);
            if (converter is SPFieldConverter spConverter)
            {
                spConverter.Web = web;
                spConverter.Field = field;

                return spConverter;
            }
            return converter;
        }

        protected object ToFieldValue(SPWeb web, SPField field, FieldToPropertyMapping mapping, object propertyValue)
        {
            var converter = GetConverter(web, field, mapping);
            return converter
                    .ConvertPropertyValueToFieldValue(mapping.PropertyInfo, propertyValue);
        }

        protected object ToPropertyValue(SPWeb web, SPField field, FieldToPropertyMapping mapping, object fieldValue)
        {
            var converter = GetConverter(web, field, mapping);
            return converter
                    .ConvertFieldValueToPropertyValue(mapping.PropertyInfo, fieldValue);
        }

        protected IEnumerable<(FieldToPropertyMapping, object)> FieldMappingsToFieldValues(SPWeb web, SPList list, BaseSPEntity entity)
        {
            foreach (var fieldMapping in FieldMappings)
            {
                if (!fieldMapping.ReadOnly)
                {
                    SPField field = list.Fields
                        .GetFieldByInternalName(fieldMapping.FieldName);

                    object propertyValue = fieldMapping
                        .PropertyInfo.GetValue(entity);

                    object fieldValue = ToFieldValue(web, field, fieldMapping, propertyValue);

                    yield return (fieldMapping, fieldValue);
                }
            }
        }

        protected IEnumerable<(FieldToPropertyMapping, object)> FieldMappingsToPropertyValues(SPWeb web, SPList list, SPListItem item)
        {
            foreach (var fieldMapping in FieldMappings)
            {
                SPField field = item.Fields
                    .GetFieldByInternalName(fieldMapping.FieldName);

                var fieldValue = item[fieldMapping.FieldName];

                var propertyValue = ToPropertyValue(web, field, fieldMapping, fieldValue);

                yield return (fieldMapping, propertyValue);
            }
        }
    }
}
