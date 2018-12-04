using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using SP2016.Repository.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using SP2016.Repository.Converters.Default;
using SP2016.Repository.Logging;
using SP2016.Repository.Models;
using SP2016.Repository.Utils;

namespace SP2016.Repository.Mapping
{
    public class SPFieldToPropertyMapper : FieldToPropertyMapper, ISPFieldToPropertyMapper
    {
        private readonly ILog logger = new Logger { ApplicationName = "SP2016.Repository.Mapping.SPFieldToPropertyMapper" };

        public SPFieldToPropertyMapper()
        {
            defaultConvertersByTypeMapping[typeof(SPPrincipal)] = new SPPrincipalConverter();
            defaultConvertersByTypeMapping[typeof(SPUser)] = new SPPrincipalConverter();
            defaultConvertersByTypeMapping[typeof(SPGroup)] = new SPPrincipalConverter();
            defaultConvertersByTypeMapping[typeof(SPPrincipalInfo)] = new SPPrincipalInfoConverter();
            defaultConvertersByTypeMapping[typeof(SPFieldLookupValue)] = new SPFieldLookupConverter();
            defaultConvertersByTypeMapping[typeof(SPFieldLookupValueCollection)] = new SPFieldLookupConverter();
            defaultConvertersByTypeMapping[typeof(SPFieldMultiChoiceValue)] = new SPFieldMultiChoiceValueConverter();
            defaultConvertersByTypeMapping[typeof(SPFieldUrlValue)] = new SPFieldUrlValueConverter();
            defaultConvertersByTypeMapping[typeof(SPFieldUserValue)] = new SPFieldUserConverter();
            defaultConvertersByTypeMapping[typeof(SPFieldUserValueCollection)] = new SPFieldUserConverter();
            defaultConvertersByTypeMapping[typeof(CalendarReccurenceData)] = new CalendarReccurenceDataConverter();
            defaultConvertersByTypeMapping[typeof(SPFieldCalculatedValue)] = new SPFieldCalculatedValueConverter();
            defaultConvertersByTypeMapping[typeof(SPContentType)] = new SPContentTypeValueConverter();
        }
        
        public SPFieldToPropertyMapper(params FieldToPropertyMapping[] mappings) 
            : this()
        {
            Mappings.AddRange(mappings);
        }

        /// <summary>
        /// Получить название поля соответствующего свойству сущности
        /// 
        /// TODO :: remove
        /// 
        /// </summary>
        /// <param name="propertyName">Свойство сущности</param>
        /// <returns>Display name поля</returns>
        public string GetFieldDisplayName(string propertyName)
        {
            string displayName = string.Empty;

            foreach (var mapping in Mappings)
            {
                if (mapping.EntityPropertyName.Equals(propertyName))
                {
                    displayName = mapping.FieldName;
                    break;
                }
            }

            if (string.IsNullOrEmpty(displayName))
                throw new InvalidOperationException($"Не найдено соответствие для свойства {propertyName}.");

            return displayName;
        }


        #region batch

        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public string GenerateBatchCommandFromEntity(SPWeb web, SPList spList, Type entityType, object entity)
        {
            StringBuilder builder = new StringBuilder();

            foreach (FieldToPropertyMapping fieldMapping in Mappings)
            {
                if (!fieldMapping.ReadOnly)
                {
                    PropertyInfo propertyInfo = ReflectionUtil.GetPropertyInfo(entityType, fieldMapping);
                    SPField field = ItemFieldsChecking.EnsureListFieldID(spList, entityType, fieldMapping);

                    object propertyValue = propertyInfo.GetValue(entity, null);
                    object fieldValue;
                    if (propertyValue == null || string.IsNullOrEmpty(propertyValue.ToString()))
                        fieldValue = null;
                    else
                        fieldValue = GetBatchFieldValuesConverter(web, propertyInfo, field).ConvertPropertyValueToFieldValue(propertyValue);

                    builder.AppendFormat("<SetVar Name=\"urn:schemas-microsoft-com:office:office#{0}\">{1}</SetVar>", fieldMapping.FieldName, fieldValue);
                }
            }

            return builder.ToString();
        }

        #endregion

        #region Item from Entity

        private IEnumerable<(string, object)> FieldNamesToValues(SPWeb web, SPList list, Type entityType, object entity)
        {
            foreach (var mapping in Mappings)
            {
                if (!mapping.ReadOnly)
                {
                    SPField field = ItemFieldsChecking.EnsureListFieldID(list, entityType, mapping);

                    PropertyInfo propertyInfo = ReflectionUtil.GetPropertyInfo(entityType, mapping);
                    object propertyValue = propertyInfo.GetValue(entity, null);
                    object fieldValue = null;

                    if (propertyValue != null && !string.IsNullOrEmpty(propertyValue.ToString()))
                    {
                        var converter = GetAfterPropertiesFieldValuesConverter(web, propertyInfo, field);
                        fieldValue = converter.ConvertPropertyValueToFieldValue(propertyValue);
                    }

                    yield return (field.InternalName, propertyValue);
                }
            }
        }

        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public void FillAfterPropertiesFromEntity(SPWeb web, SPItemEventProperties properties, Type entityType, object entity)
        {
            var fieldNamesToValues = FieldNamesToValues(web, properties.List, entityType, entity);
            
            foreach (var fieldNameToValue in fieldNamesToValues)
            {
                (string fieldName, object fieldValue) = fieldNameToValue;

                try
                {
                    properties.AfterProperties[fieldName] = fieldValue;
                }
                catch (Exception ex)
                {
                    string errorMessage = $@"Не удалось записать значение {fieldValue} в поле {fieldName} 
                            для сущности {entityType.FullName}.";
                    var nextException = new Exception(errorMessage, ex);
                    logger.Error(errorMessage, nextException);
                    throw nextException;
                }
            }
        }
        
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public void FillSPListItemFromEntity(SPWeb web, SPListItem spListItem, Type entityType, object entity)
        {
            var fieldNamesToValues = FieldNamesToValues(web, spListItem.ParentList, entityType, entity);

            foreach (var fieldNameToValue in fieldNamesToValues)
            {
                (string fieldName, object fieldValue) = fieldNameToValue;

                try
                {
                    spListItem[fieldName] = fieldValue;
                }
                catch (Exception ex)
                {
                    string errorMessage = $@"Не удалось записать значение {fieldValue} в поле {fieldName} 
                                для сущности {entityType.FullName}.";
                    var nextException = new Exception(errorMessage, ex);
                    logger.Error(errorMessage, nextException);
                    throw nextException;
                }
            }
        }

        #endregion

        #region Entity from Item

        public void FillEntityFromSPListItem(SPWeb web, object entity, Type entityType, SPListItem item)
        {
            foreach (FieldToPropertyMapping fieldMapping in Mappings)
            {
                try
                {
                    PropertyInfo propertyInfo = ReflectionUtil.GetPropertyInfo(item, entityType, fieldMapping);

                    // TODO :: Check field exists
                    //ItemFieldsChecking.EnsureListFieldID(item, entityType, fieldMapping);

                    SPField field = item.Fields.GetField(fieldMapping.FieldName);
                    object fieldValue = item[fieldMapping.FieldName];

                    if (fieldValue is null || string.IsNullOrEmpty(fieldValue.ToString()))
                        propertyInfo.SetValue(entity, null);
                    else
                    {
                        var converter = GetDefaultConverter(web, propertyInfo, field);
                        var value = converter.ConvertFieldValueToPropertyValue(fieldValue);
                        propertyInfo.SetValue(entity, value);
                    }
                }
                catch (Exception ex)
                {
                    string messageFormat = "Ошибка сопоставления свойства {0} сущности {1} с полем {2} элемента {3} списка {4} узла {5}";
                    string message = string.Format(messageFormat,
                        fieldMapping.EntityPropertyName,
                        entityType.Name,
                        fieldMapping.FieldName,
                        item.ID,
                        item.ParentList.Title,
                        web.Title);

                    InvalidOperationException newException = new InvalidOperationException(message, ex);
                    logger.Error(message, newException);
                    throw newException;
                }
            }
        }

        public void FillEntityFromAfterProperties(SPWeb web, object entity, Type entityType, SPItemEventProperties properties)
        {
            SPList list = web.Lists[properties.ListId];
            foreach (var fieldMapping in Mappings)
            {

                try
                {
                    PropertyInfo propertyInfo = ReflectionUtil.GetPropertyInfo(entityType, fieldMapping);

                    SPField field = list.Fields.GetField(fieldMapping.FieldName);
                    object fieldValue;
                    object value;
                    if (properties.AfterProperties.Cast<DictionaryEntry>().Any(e => e.Key.Equals(field.InternalName)))
                    {
                        fieldValue = properties.AfterProperties[field.InternalName];

                        if (fieldValue == null || string.IsNullOrEmpty(fieldValue.ToString()))
                            value = null;
                        else
                            value = GetAfterPropertiesFieldValuesConverter(web, propertyInfo, field).ConvertFieldValueToPropertyValue(fieldValue);
                    }
                    else
                    {
                        fieldValue = properties.ListItem != null ? properties.ListItem[fieldMapping.FieldName] : null;

                        if (fieldValue == null || string.IsNullOrEmpty(fieldValue.ToString()))
                            value = null;
                        else
                            value = GetDefaultConverter(web, propertyInfo, field).ConvertFieldValueToPropertyValue(fieldValue);
                    }

                    propertyInfo.SetValue(entity, value, null);
                }
                catch (Exception ex)
                {
                    string messageFormat = "Ошибка сопоставления свойства {0} сущности {1} с полем {2} элемента списка {3} узла {4}";
                    string message = string.Format(messageFormat,
                        fieldMapping.EntityPropertyName,
                        entityType.Name,
                        fieldMapping.FieldName,
                        list.Title,
                        web.Title);

                    InvalidOperationException newException = new InvalidOperationException(message, ex);
                    logger.Error(message, newException);
                    throw newException;
                }
            }
        }

        #endregion




        public SimpleConverter GetDefaultConverter(SPWeb web, PropertyInfo propertyInfo, SPField field)
        {
            return GetFieldValuesConverter(web, propertyInfo, field);
        }




        public SimpleConverter GetBatchFieldValuesConverter(SPWeb web, PropertyInfo propertyInfo, SPField field)
        {
            Dictionary<Type, SimpleConverter> BatchConvertersMapping = new Dictionary<Type, SimpleConverter>
            {
                { typeof(DateTime), new XmlDateTimeFieldValueConverter() },
                { typeof(SPContentTypeId), new SPContentTypeIdValueConverter() },
                { typeof(SPFieldUserValueCollection), new SPFieldUserValueCollectionConverter() },
            };

            return GetFieldValuesConverter(web, propertyInfo, field, BatchConvertersMapping);
        }

        public SimpleConverter GetAfterPropertiesFieldValuesConverter(SPWeb web, PropertyInfo propertyInfo, SPField field)
        {
            Dictionary<Type, SimpleConverter> AfterPropertiesConvertersMapping = new Dictionary<Type, SimpleConverter>
            {
                { typeof(DateTime), new XmlDateTimeFieldValueConverter() },
                { typeof(SPContentTypeId), new SPContentTypeIdValueConverter() },
                { typeof(SPFieldUserValueCollection), new SPFieldUserValueCollectionConverter() },
            };

            return GetFieldValuesConverter(web, propertyInfo, field, AfterPropertiesConvertersMapping);
        }

        public SimpleConverter GetItemVersionFieldValuesConverter(SPWeb web, PropertyInfo propertyInfo, SPField field)
        {
            Dictionary<Type, SimpleConverter> ItemVersionConvertersMapping = new Dictionary<Type, SimpleConverter>
            {
                { typeof(DateTime), new ItemVersionDateTimeConverter() }
            };

            return GetFieldValuesConverter(web, propertyInfo, field, ItemVersionConvertersMapping);
        }




        private SimpleConverter GetFieldValuesConverter(SPWeb web, PropertyInfo propertyInfo, SPField field, Dictionary<Type, SimpleConverter> customConvertersMapping = null)
        {
            //get custom converter
            var converter = GetConverterForProperty(propertyInfo, customConvertersMapping);

            if (converter is BaseSharePointConverter)
            {
                var sharePointConverter = (BaseSharePointConverter)converter;
                sharePointConverter.FieldInfo = field;
                sharePointConverter.Web = web;
            }
            return converter;
        }
    }
}
