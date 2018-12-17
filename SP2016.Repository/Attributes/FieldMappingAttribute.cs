using SP2016.Repository.Converters;
using System;

namespace SP2016.Repository.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FieldMappingAttribute : Attribute
    {
        public bool IsReadOnly { get; }

        public string FieldName { get; }

        public FieldConverter Converter { get; }

        public FieldMappingAttribute(string fieldName)
        {
            FieldName = fieldName;
        }

        public FieldMappingAttribute(string fieldName, bool isReadOnly = false) 
            : this(fieldName)
        {
            IsReadOnly = isReadOnly;
        }

        public FieldMappingAttribute(string fieldName, FieldConverter converter, bool isReadOnly = false) 
            : this(fieldName, isReadOnly)
        {
            Converter = converter;
        }
    }
}
