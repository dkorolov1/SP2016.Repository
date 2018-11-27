using SP2016.Repository.Attributes;
using SP2016.Repository.Converters.Default;
using SP2016.Repository.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace SP2016.Repository.Mapping
{
    public abstract class FieldToPropertyMapper : IListItemFieldMapper
    {
        /// <summary>
        /// Коллекция сопоставления полей элементов списка и свойств сущности
        /// </summary>
        protected readonly List<FieldToEntityPropertyMapping> fieldMappings = new List<FieldToEntityPropertyMapping>();

        protected readonly Dictionary<string, BaseConverter> defaultConvertersByNameMapping = new Dictionary<string, BaseConverter>();

        public IReadOnlyCollection<FieldToEntityPropertyMapping> Mappings => fieldMappings;

        public void AddMapping(FieldToEntityPropertyMapping fieldToPropertyMapRecord)
        {
            fieldMappings.Add(fieldToPropertyMapRecord);
        }

        public void AddMapping(FieldToEntityPropertyMapping[] fieldToPropertyMapRecords)
        {
            fieldMappings.AddRange(fieldToPropertyMapRecords);
        }

        public void RegisterConverterForPropertyName(string propertyName, BaseConverter converter, Dictionary<string, BaseConverter> customConvertersMapping = null)
        {
            (customConvertersMapping ?? defaultConvertersByNameMapping).Add(propertyName, converter);
        }

        public void RegisterConverterForPropertyType(Type propertyType, BaseConverter converter, Dictionary<Type, BaseConverter> customConvertersMapping = null)
        {
            (customConvertersMapping ?? defaultConvertersByTypeMapping).Add(propertyType, converter);
        }

        public virtual BaseConverter GetConverterForProperty(string propertyName, Type entityType, Dictionary<Type, BaseConverter> customConvertersMapping = null)
        {
            var propertyInfo = ReflectionUtil.GetPropertyInfo(entityType, propertyName);
            return GetConverterForProperty(propertyInfo, customConvertersMapping);
        }

        public virtual BaseConverter GetConverterForProperty(PropertyInfo propertyInfo, Dictionary<Type, BaseConverter> customConvertersMapping = null)
        {
            if (defaultConvertersByNameMapping.ContainsKey(propertyInfo.Name))
                return defaultConvertersByNameMapping[propertyInfo.Name];

            BaseConverter converter = TryGetConverterFromAttribute(propertyInfo) ?? GetConverterByPropertyType(propertyInfo, customConvertersMapping);

            if (converter != null)
                converter.PropertyInfo = propertyInfo;

            return converter;
        }

        protected BaseConverter TryGetConverterFromAttribute(PropertyInfo propertyInfo)
        {
            object[] specialConverterAttributes = propertyInfo.GetCustomAttributes(typeof(FieldMappingAttribute), true);

            if (specialConverterAttributes.Length == 0)
                return null;

            if (specialConverterAttributes.Length > 1)
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, "Более одного конвертера для одного свойства {0} класса {1}", propertyInfo.Name, propertyInfo.DeclaringType.Name));

            FieldMappingAttribute fieldValuesConverterAttribute = (FieldMappingAttribute)specialConverterAttributes[0];
            if (string.IsNullOrWhiteSpace(fieldValuesConverterAttribute.FieldValuesConverterTypeName))
                return null;

            Type converterType = Type.GetType(fieldValuesConverterAttribute.FieldValuesConverterTypeName);
            BindingFlags bindingAttr = BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance;
            var converter = Activator.CreateInstance(converterType, bindingAttr, null, null, null) as BaseConverter;

            if (converter == null)
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, "Конвертер для свойства {0} класса {1} не указан или не реализует необходимый интерфейс", propertyInfo.Name, propertyInfo.DeclaringType.Name));

            return converter;
        }

        private BaseConverter GetConverterByPropertyType(PropertyInfo propertyInfo, Dictionary<Type, BaseConverter> customConvertersMapping)
        {
            var propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

            if (propertyType.IsEnum)
                propertyType = typeof(Enum);

            var converter = TryGetConverter(propertyType, customConvertersMapping)
                ?? TryGetConverter(propertyType, defaultConvertersByTypeMapping)
                ?? new BaseConverter();
            return converter;
        }

        //Получаем конвертер по типу свойства. Поиск конвертера происходит сначала в customConvertersMapping, затем в defaultConvertersByTypeMapping
        protected BaseConverter TryGetConverter(Type propertyType, Dictionary<Type, BaseConverter> customConvertersMapping)
        {
            BaseConverter converter = null;
            if (customConvertersMapping?.ContainsKey(propertyType) == true)
                converter = customConvertersMapping[propertyType];
            if (converter == null && defaultConvertersByTypeMapping.ContainsKey(propertyType))
                converter = defaultConvertersByTypeMapping[propertyType];
            return converter;
        }

        //конвертеры по умолчанию
        protected readonly Dictionary<Type, BaseConverter> defaultConvertersByTypeMapping = new Dictionary<Type, BaseConverter>
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
    }
}
