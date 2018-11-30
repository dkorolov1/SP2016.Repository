using SP2016.Repository.Attributes;
using SP2016.Repository.Converters.Default;
using SP2016.Repository.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SP2016.Repository.Mapping
{
    public abstract class FieldToPropertyMapper
    {
        protected List<FieldToPropertyMapping> Mappings { get; }

        public FieldToPropertyMapper()
        {
            Mappings = new List<FieldToPropertyMapping>();
        }

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

        // TODO :: move code related to reflection to reflection util
        protected BaseConverter TryGetConverterFromAttribute(PropertyInfo propertyInfo)
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
            var converter = Activator.CreateInstance(converterType, bindingAttr, null, null, null) as BaseConverter;

            if (converter == null)
            {
                string errorMessage = $@"Конвертер для свойства {propertyInfo.Name} класса 
                            {propertyInfo.DeclaringType.Name} не указан или не реализует необходимый интерфейс";
                   
                throw new InvalidOperationException(errorMessage);
            }

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
            BaseConverter converter = TryGetConverterFromAttribute(propertyInfo) ?? 
                GetConverterByPropertyType(propertyInfo, customConvertersMapping);

            if (converter != null)
                converter.PropertyInfo = propertyInfo;

            return converter;
        }
    }
}
