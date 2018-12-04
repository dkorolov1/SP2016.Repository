using Microsoft.SharePoint;
using SP2016.Repository.Attributes;
using SP2016.Repository.Converters;
using SP2016.Repository.Converters.Default;
using SP2016.Repository.Entities;
using SP2016.Repository.Logging;
using SP2016.Repository.Models;
using SP2016.Repository.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SP2016.Repository.Mapping
{
    public class SPListItemVersionMapper<TEntity> : SharePointFieldMapper<TEntity> where TEntity : BaseEntity, new()
    {


        public void Map(SPWeb web, object to, SPListItemVersion from)
        {
            
        }
    }

    public class SPListItemMapper<TEntity> : SharePointFieldMapper<TEntity> where TEntity : BaseEntity, new()
    {
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
                    logger.Error(errorMessage, nextException);
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

    public abstract class SharePointFieldMapper<TEntity> : FieldMapper where TEntity : BaseEntity, new()
    {
        protected override Dictionary<Type, SimpleConverter> UniqueConverters => new Dictionary<Type, SimpleConverter>()
        {
            [typeof(SPPrincipal)] = new StringValueConverter(),
            [typeof(SPUser)] = new StringValueConverter(),
            [typeof(SPGroup)] = new StringValueConverter(),
            [typeof(SPPrincipalInfo)] = new StringValueConverter(),
            [typeof(SPFieldLookupValue)] = new StringValueConverter(),
            [typeof(SPFieldLookupValueCollection)] = new StringValueConverter(),
            [typeof(SPFieldMultiChoiceValue)] = new StringValueConverter(),
            [typeof(SPFieldUrlValue)] = new StringValueConverter(),
            [typeof(SPFieldUserValue)] = new StringValueConverter(),
            [typeof(SPFieldUserValueCollection)] = new StringValueConverter(),
            [typeof(CalendarReccurenceData)] = new StringValueConverter(),
            [typeof(SPFieldCalculatedValue)] = new StringValueConverter(),
            [typeof(SPContentType)] = new StringValueConverter(),
        };

        protected IEnumerable<(FieldToPropertyMapping, object)> FieldMappingsToFieldValues(SPWeb web, SPList list, object entity)
        {
            foreach (var mapping in Mappings)
            {
                if (!mapping.ReadOnly)
                {
                    SPField field = list.Fields.GetFieldByInternalName(mapping.FieldName);

                    PropertyInfo propertyInfo = ReflectionUtil.GetPropertyInfo(typeof(TEntity), mapping);
                    object propertyValue = propertyInfo.GetValue(entity);

                    if (propertyValue is null)
                        yield return (mapping, null);

                    object fieldValue = null;
                    var converter = GetConverter(propertyInfo);

                    switch (converter)
                    {
                        case SharePointConverter spConverter:
                            {
                                fieldValue = spConverter
                                    .ConvertPropertyValueToFieldValue(web, propertyInfo, propertyValue);
                            }
                            break;
                        case SimpleConverter simpleConverter:
                            {
                                fieldValue = simpleConverter
                                    .ConvertPropertyValueToFieldValue(propertyInfo, fieldValue);
                            }
                            break;
                    }

                    yield return (mapping, propertyValue);
                }
            }
        }

        protected IEnumerable<(FieldToPropertyMapping, object)> FieldMappingsToPropertyValues(SPWeb web, SPList list, SPListItem item)
        {
            foreach (FieldToPropertyMapping fieldMapping in Mappings)
            {
                PropertyInfo propertyInfo = ReflectionUtil.GetPropertyInfo(item, typeof(TEntity), fieldMapping);
                SPField field = item.Fields.GetField(fieldMapping.FieldName);
                var fieldValue = item[fieldMapping.FieldName];

                if (fieldValue is null)
                    yield return (fieldMapping, null);

                var converter = GetConverter(propertyInfo);
                switch (converter)
                {
                    case SharePointConverter spConverter:
                        {
                            fieldValue = spConverter
                                .ConvertFieldValueToPropertyValue(web, propertyInfo, fieldValue);
                        }
                        break;
                    case SimpleConverter simpleConverter:
                        {
                            fieldValue = simpleConverter
                                .ConvertFieldValueToPropertyValue(propertyInfo, fieldValue);
                        }
                        break;
                }

                yield return (fieldMapping, fieldValue);
            }
        }
    }




    public abstract class FieldMapper
    {
        protected readonly ILog logger = new Logger { ApplicationName = "SP2016.Repository.Mapping.FieldMapper" };

        protected virtual List<FieldToPropertyMapping> Mappings => new List<FieldToPropertyMapping>();
        protected virtual Dictionary<Type, SimpleConverter> UniqueConverters => new Dictionary<Type, SimpleConverter>();

        private readonly Dictionary<Type, SimpleConverter> converters = new Dictionary<Type, SimpleConverter>();

        public FieldMapper()
        {
            converters = new Dictionary<Type, SimpleConverter>
            {
                [typeof(string)] = new StringValueConverter(),
                [typeof(int)] = new Int32Converter(),
                [typeof(float)] = new SingleConverter(),
                [typeof(double)] = new DoubleConverter(),
                [typeof(DateTime)] = new DateTimeConverter(),
                [typeof(bool)] = new BooleanConverter(),
                [typeof(Enum)] = new EnumConverter(),
                [typeof(Guid)] = new GuidConverter(),
            };

            // TODO :: merge converters here

        }

        private SimpleConverter TryGetConverterFromAttribute(PropertyInfo propertyInfo)
        {
            object[] specialConverterAttributes = propertyInfo.GetCustomAttributes(typeof(FieldMappingAttribute), true);

            if (specialConverterAttributes.Length == 0)
                return null;

            if (specialConverterAttributes.Length > 1)
            {
                string errorMessage = $@"Более одного конвертера для одного свойства {propertyInfo.Name} 
                        класса {propertyInfo.DeclaringType.Name}";

                throw new InvalidOperationException(errorMessage);
            }

            FieldMappingAttribute fieldValuesConverterAttribute = (FieldMappingAttribute)specialConverterAttributes[0];
            if (string.IsNullOrWhiteSpace(fieldValuesConverterAttribute.FieldValuesConverterTypeName))
                return null;

            Type converterType = Type.GetType(fieldValuesConverterAttribute.FieldValuesConverterTypeName);
            BindingFlags bindingAttr = BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance;
            var converter = Activator.CreateInstance(converterType, bindingAttr, null, null, null) as SimpleConverter;

            if (converter == null)
            {
                string errorMessage = $@"Конвертер для свойства {propertyInfo.Name} класса 
                            {propertyInfo.DeclaringType.Name} не указан или не реализует необходимый интерфейс";

                throw new InvalidOperationException(errorMessage);
            }

            return converter;
        }

        private SimpleConverter GetConverterByPropertyType(PropertyInfo propertyInfo)
        {
            var propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

            if (propertyType.IsEnum)
                propertyType = typeof(Enum);

            SimpleConverter converter = new SimpleConverter();

            if (converters.ContainsKey(propertyType))
                converter = converters[propertyType];

            return converter;
        }

        protected IConverter GetConverter(PropertyInfo propertyInfo)
        {
            return TryGetConverterFromAttribute(propertyInfo) ??
                GetConverterByPropertyType(propertyInfo);
        }
    }
}
