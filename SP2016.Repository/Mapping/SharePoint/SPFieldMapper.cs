using Microsoft.SharePoint;
using SP2016.Repository.Converters;
using SP2016.Repository.Converters.Common;
using SP2016.Repository.Converters.SharePoint;
using SP2016.Repository.Entities;
using SP2016.Repository.Models;
using SP2016.Repository.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SP2016.Repository.Mapping.SharePoint
{
    public abstract class SPFieldMapper<TEntity> : FieldMapper where TEntity : BaseEntity
    {
        public SPFieldMapper(IEnumerable<FieldToPropertyMapping> mappings) 
            : base(mappings)
        {
            var spConverters = new (Type, IConverter)[] {
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
                (typeof(SPContentType), new SPContentTypeValueConverter())
            };

            RegisterUniqueConverters(spConverters);
        }

        protected IEnumerable<(FieldToPropertyMapping, object)> FieldMappingsToFieldValues(SPWeb web, SPList list, BaseEntity entity)
        {
            foreach (var mapping in FieldMappings)
            {
                if (!mapping.ReadOnly)
                {
                    PropertyInfo propertyInfo = ReflectionUtil.GetPropertyInfo(typeof(TEntity), mapping);
                    SPField field = list.Fields.GetFieldByInternalName(mapping.FieldName);

                    object propertyValue = propertyInfo.GetValue(entity);
                    object fieldValue = ConvertPropertyValueToFieldValue(web, field, propertyInfo, propertyValue);

                    yield return (mapping, fieldValue);
                }
            }
        }

        protected IEnumerable<(FieldToPropertyMapping, object)> FieldMappingsToPropertyValues(SPWeb web, SPList list, SPListItem item)
        {
            foreach (FieldToPropertyMapping fieldMapping in FieldMappings)
            {
                PropertyInfo propertyInfo = ReflectionUtil.GetPropertyInfo(typeof(TEntity), fieldMapping);
                SPField field = item.Fields.GetField(fieldMapping.FieldName);

                var fieldValue = item[fieldMapping.FieldName];
                var propertyValue = ConvertFieldValueToPropertyValue(web, field, propertyInfo, fieldValue);

                yield return (fieldMapping, propertyValue);
            }
        }

        protected object ConvertPropertyValueToFieldValue(SPWeb web, SPField field, PropertyInfo propertyInfo, object propertyValue)
        {
            if (propertyValue is null)
                return null;

            var converter = GetConverter(propertyInfo);

            switch (converter)
            {
                case SharePointConverter spConverter:
                    {
                        return spConverter
                            .ConvertPropertyValueToFieldValue(web, field, propertyInfo, propertyValue);
                    }
                case SimpleConverter simpleConverter:
                    {
                        return simpleConverter
                            .ConvertPropertyValueToFieldValue(propertyInfo, propertyValue);
                    }
                default:
                    return null;
            }
        }

        protected object ConvertFieldValueToPropertyValue(SPWeb web, SPField field, PropertyInfo propertyInfo, object fieldValue)
        {
            if (fieldValue is null)
                return null;

            var converter = GetConverter(propertyInfo);

            switch (converter)
            {
                case SharePointConverter spConverter:
                    {
                        return spConverter
                            .ConvertFieldValueToPropertyValue(web, field, propertyInfo, fieldValue);
                    }
                case SimpleConverter simpleConverter:
                    {
                        return simpleConverter
                            .ConvertFieldValueToPropertyValue(propertyInfo, fieldValue);
                    }
                default:
                    return null;
            }
        }
    }
}
