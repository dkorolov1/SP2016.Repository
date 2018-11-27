using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using SP2016.Repository.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
        
        public SPFieldToPropertyMapper(FieldToEntityPropertyMapping[] mappings) : this()
        {
            AddMapping(mappings);
        }

        /// <summary>
        /// Получить название поля соответствующего свойству сущности
        /// </summary>
        /// <param name="propertyName">Свойство сущности</param>
        /// <returns>Display name поля</returns>
        public string GetFieldDisplayName(string propertyName)
        {
            string displayName = string.Empty;
            foreach (FieldToEntityPropertyMapping mapping in fieldMappings)
            {
                if (mapping.EntityPropertyName.Equals(propertyName))
                {
                    displayName = mapping.FieldName;
                    break;
                }
            }
            if (string.IsNullOrEmpty(displayName))
                throw new InvalidOperationException(string.Format("Не найдено соответствие для свойства {0}.", propertyName));

            return displayName;
        }

        /// <summary>
        /// Получить название свойства сущности соответствующего имени поля.
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public string GetPropertyNameByFieldDisplayName(string displayName)
        {
            string propertyName = string.Empty;
            foreach (FieldToEntityPropertyMapping mapping in fieldMappings)
            {
                if (mapping.FieldName.Equals(displayName))
                {
                    propertyName = mapping.EntityPropertyName;
                    break;
                }
            }
            if (string.IsNullOrEmpty(propertyName))
                throw new InvalidOperationException(string.Format("Не найдено соответствие для поля {0}.", displayName));

            return propertyName;
        }

        #region Entity <-> Item

        /// <summary>
        /// Generate batch command based on the values in an entity.
        /// </summary>
        /// <param name="spList">Список</param>
        /// <param name="entity">Сущность с данными</param>
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public string GenerateBatchCommandFromEntity(SPWeb web, SPList spList, Type entityType, object entity)
        {
            StringBuilder builder = new StringBuilder();

            foreach (FieldToEntityPropertyMapping fieldMapping in fieldMappings)
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

        public void FillEntityFromAfterProperties(SPWeb web, object entity, Type entityType, SPItemEventProperties properties)
        {
            SPList list = web.Lists[properties.ListId];
            foreach (FieldToEntityPropertyMapping fieldMapping in this.fieldMappings)
            {

                try
                {
                    PropertyInfo propertyInfo = ReflectionUtil.GetPropertyInfo(entityType, fieldMapping);
                    //EnsureListFieldID(item, entityType, fieldMapping);

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

        /// <summary>
        /// Заполнить AfterProperties на основе данных сущности
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="entity">Сущность с данными</param>
        [SharePointPermissionAttribute(SecurityAction.LinkDemand, ObjectModel = true)]
        public void FillAfterPropertiesFromEntity(SPWeb web, SPItemEventProperties properties, Type entityType, object entity)
        {
            //Type entityType = typeof(TEntity);
            foreach (FieldToEntityPropertyMapping fieldMapping in this.fieldMappings)
            {
                if (!fieldMapping.ReadOnly)
                {
                    PropertyInfo propertyInfo = ReflectionUtil.GetPropertyInfo(entityType, fieldMapping);
                    SPField field = ItemFieldsChecking.EnsureListFieldID(properties.List, entityType, fieldMapping);

                    object propertyValue = propertyInfo.GetValue(entity, null);
                    object fieldValue;

                    if (propertyValue == null || string.IsNullOrEmpty(propertyValue.ToString()))
                        fieldValue = null;
                    else
                        fieldValue = GetAfterPropertiesFieldValuesConverter(web, propertyInfo, field).ConvertPropertyValueToFieldValue(propertyValue);

                    try
                    {
                        properties.AfterProperties[fieldMapping.FieldName] = fieldValue;
                    }
                    catch (ArgumentException argumentException)
                    {
                        string errorMessage = string.Format(
                            CultureInfo.CurrentCulture,
                            "Не удалось записать значение {0} в поле {1} для сущности {2}.",
                            fieldValue,
                            fieldMapping.FieldName,
                            entityType.FullName);

                        var newException = new ListItemFieldMappingException(errorMessage, argumentException);
                        logger.Error(errorMessage, newException);
                        throw newException;
                    }
                }
            }
        }

        /// <summary>
        /// Заполнить SPListItem на основе данных сущности
        /// </summary>
        /// <param name="spListItem">Элемент списка</param>
        /// <param name="entity">Сущность с данными</param>
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public void FillSPListItemFromEntity(SPWeb web, SPListItem spListItem, Type entityType, object entity)
        {
            //Type entityType = typeof(TEntity);
            foreach (FieldToEntityPropertyMapping fieldMapping in this.fieldMappings)
            {
                if (!fieldMapping.ReadOnly)
                {
                    PropertyInfo propertyInfo = ReflectionUtil.GetPropertyInfo(spListItem, entityType, fieldMapping);
                    SPField field = ItemFieldsChecking.EnsureListFieldID(spListItem, entityType, fieldMapping);

                    object propertyValue = propertyInfo.GetValue(entity, null);
                    object fieldValue;

                    if (propertyValue == null || string.IsNullOrEmpty(propertyValue.ToString()))
                        fieldValue = null;
                    else
                        fieldValue = GetDefaultConverter(web, propertyInfo, field).ConvertPropertyValueToFieldValue(propertyValue);

                    try
                    {
                        spListItem[fieldMapping.FieldName] = fieldValue;
                    }
                    catch (ArgumentException argumentException)
                    {
                        string errorMessage = string.Format(
                            CultureInfo.CurrentCulture,
                            "Не удалось записать значение {0} в поле {1} для сущности {2}.",
                            fieldValue,
                            fieldMapping.FieldName,
                            entityType.FullName);

                        var newException = new ListItemFieldMappingException(errorMessage, argumentException);
                        logger.Error(errorMessage, newException);
                        throw newException;
                    }
                }
            }
        }

        public void FillEntityFromSPListItem(SPWeb web, object entity, Type entityType, SPListItem item)
        {
            foreach (FieldToEntityPropertyMapping fieldMapping in this.fieldMappings)
            {
                try
                {
                    PropertyInfo propertyInfo = ReflectionUtil.GetPropertyInfo(item, entityType, fieldMapping);
                    ItemFieldsChecking.EnsureListFieldID(item, entityType, fieldMapping);

                    SPField field = item.Fields.GetField(fieldMapping.FieldName);
                    object fieldValue = item[fieldMapping.FieldName];
                    object value;

                    if (fieldValue == null || string.IsNullOrEmpty(fieldValue.ToString()))
                        value = null;
                    else
                    {
                        var converter = GetDefaultConverter(web, propertyInfo, field);
                        value = converter.ConvertFieldValueToPropertyValue(fieldValue);
                    }
                    propertyInfo.SetValue(entity, value, null);
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

        public void FillEntityFromSPListItem(SPWeb web, object entity, Type entityType, SPListItemVersion item)
        {
            foreach (FieldToEntityPropertyMapping fieldMapping in this.fieldMappings)
            {
                try
                {
                    PropertyInfo propertyInfo = ReflectionUtil.GetPropertyInfo(item, entityType, fieldMapping);
                    ItemFieldsChecking.EnsureListFieldID(item, entityType, fieldMapping);

                    SPField field = item.Fields.GetField(fieldMapping.FieldName);
                    object fieldValue = item[fieldMapping.FieldName];
                    object value;

                    if (fieldValue == null || string.IsNullOrEmpty(fieldValue.ToString()))
                        value = null;
                    else
                        value = GetItemVersionFieldValuesConverter(web, propertyInfo, field).ConvertFieldValueToPropertyValue(fieldValue);

                    propertyInfo.SetValue(entity, value, null);
                }
                catch (Exception ex)
                {
                    string messageFormat = "Ошибка сопоставления свойства {0} сущности {1} с полем {2} версии {3} элемента {4} списка {5} узла {6}";
                    string message = string.Format(messageFormat,
                        fieldMapping.EntityPropertyName,
                        entityType.Name,
                        fieldMapping.FieldName,
                        item.VersionId,
                        item.ListItem.ID,
                        item.ListItem.ParentList.Title,
                        web.Title);

                    InvalidOperationException newException = new InvalidOperationException(message, ex);
                    logger.Error(message, newException);
                    throw newException;
                }
            }
        }

        #endregion

        public virtual BaseConverter GetDefaultConverter(SPWeb web, PropertyInfo propertyInfo, SPField field)
        {
            return GetFieldValuesConverter(web, propertyInfo, field);
        }

        public BaseConverter GetBatchFieldValuesConverter(SPWeb web, PropertyInfo propertyInfo, SPField field)
        {
            return GetFieldValuesConverter(web, propertyInfo, field, BatchConvertersMapping);
        }

        public BaseConverter GetAfterPropertiesFieldValuesConverter(SPWeb web, PropertyInfo propertyInfo, SPField field)
        {
            return GetFieldValuesConverter(web, propertyInfo, field, AfterPropertiesConvertersMapping);
        }

        public BaseConverter GetItemVersionFieldValuesConverter(SPWeb web, PropertyInfo propertyInfo, SPField field)
        {
            return GetFieldValuesConverter(web, propertyInfo, field, ItemVersionConvertersMapping);
        }

        private BaseConverter GetFieldValuesConverter(SPWeb web, PropertyInfo propertyInfo, SPField field, Dictionary<Type, BaseConverter> customConvertersMapping = null)
        {
            var converter = GetConverterForProperty(propertyInfo, customConvertersMapping);
            if (converter is BaseSharePointConverter)
            {
                var sharePointConverter = (BaseSharePointConverter)converter;
                sharePointConverter.FieldInfo = field;
                sharePointConverter.Web = web;
            }
            return converter;
        }

        private readonly Dictionary<Type, BaseConverter> ItemVersionConvertersMapping = new Dictionary<Type, BaseConverter>
        {
            { typeof(DateTime), new ItemVersionDateTimeConverter() }
        };

        private readonly Dictionary<Type, BaseConverter> AfterPropertiesConvertersMapping = new Dictionary<Type, BaseConverter>
        {
            { typeof(DateTime), new XmlDateTimeFieldValueConverter() },
            { typeof(SPContentTypeId), new SPContentTypeIdValueConverter() },
            { typeof(SPContentType), new SPContentTypeValueConverter() },
            { typeof(SPFieldUserValueCollection), new SPFieldUserValueCollectionConverter() },
            { typeof(String), new AfterPropertiesStringValueConverter() },
        };

        private readonly Dictionary<Type, BaseConverter> BatchConvertersMapping = new Dictionary<Type, BaseConverter>
        {
            { typeof(DateTime), new XmlDateTimeFieldValueConverter() },
            { typeof(SPContentTypeId), new SPContentTypeIdValueConverter() },
            { typeof(SPContentType), new SPContentTypeValueConverter() },
            { typeof(SPFieldUserValueCollection), new SPFieldUserValueCollectionConverter() },
            { typeof(String), new BatchStringValueConverter() },
        };
    }
}
