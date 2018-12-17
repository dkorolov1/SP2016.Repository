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
        protected ILog Logger { get; }
        protected IReadOnlyCollection<FieldToPropertyMapping> FieldMappings { get; }
        private Dictionary<Type, FieldConverter> converters;

        public FieldMapper(IEnumerable<FieldToPropertyMapping> mappings)
        {
            Logger = new Logger { ApplicationName = "SP2016.Repository.Mapping.FieldMapper" };
            FieldMappings = new ReadOnlyCollection<FieldToPropertyMapping>(mappings.ToArray());

            converters = new Dictionary<Type, FieldConverter>
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

        protected void RegisterConverters(params (Type, FieldConverter)[] newConverters)
        {
            foreach (var newConverter in newConverters)
            {
                (Type type, FieldConverter converter) = newConverter;

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

        private FieldConverter GetConverterByPropertyType(PropertyInfo propertyInfo)
        {
            var propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) 
                ?? propertyInfo.PropertyType;

            if (propertyType.IsEnum)
                propertyType = typeof(Enum);

            converters
                .TryGetValue(propertyType, out FieldConverter converter);

            return converter;
        }

        protected FieldConverter GetConverter(FieldToPropertyMapping mapping)
        {
            FieldConverter converter = mapping.Converter 
                ?? GetConverterByPropertyType(mapping.PropertyInfo);

            if (converter is null)
            {
                converter = new FieldConverter();
            }

            return converter;
        }
    }
}
