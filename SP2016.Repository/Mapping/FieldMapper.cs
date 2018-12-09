using SP2016.Repository.Attributes;
using SP2016.Repository.Converters;
using SP2016.Repository.Converters.Common;
using SP2016.Repository.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace SP2016.Repository.Mapping
{
    public abstract class FieldMapper
    {
        protected readonly ILog Logger;
        protected readonly IReadOnlyCollection<FieldToPropertyMapping> FieldMappings;

        private readonly Dictionary<Type, IConverter> converters;

        public FieldMapper(IEnumerable<FieldToPropertyMapping> mappings)
        {
            Logger = new Logger { ApplicationName = "SP2016.Repository.Mapping.FieldMapper" };
            FieldMappings = new ReadOnlyCollection<FieldToPropertyMapping>(mappings.ToArray());

            converters = new Dictionary<Type, IConverter>
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

        protected void RegisterUniqueConverters(params (Type, IConverter)[] convertersInfo)
        {
            foreach (var converterInfo in convertersInfo)
            {
                (Type type, IConverter converter) = converterInfo;

                if (converters.ContainsKey(type))
                {
                    converters[type] = converter;
                }
                else
                {
                    converters.Add(type, converter);
                }
            }
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

        private IConverter GetConverterByPropertyType(PropertyInfo propertyInfo)
        {
            var propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

            if (propertyType.IsEnum)
                propertyType = typeof(Enum);

            IConverter converter = new SimpleConverter();

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
