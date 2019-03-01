using System;

namespace SP2016.Repository.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FieldMappingAttribute : Attribute
    {
        public bool IsReadOnly { get; }

        public string FieldName { get; }

        public Type Converter { get; }

        public FieldMappingAttribute(string fieldName)
        {
            FieldName = fieldName;
        }

        public FieldMappingAttribute(string fieldName, bool isReadOnly = false)
            : this(fieldName)
        {
            IsReadOnly = isReadOnly;
        }

        public FieldMappingAttribute(string fieldName, Type converter, bool isReadOnly = false) 
            : this(fieldName, isReadOnly)
        {
            Converter = converter;
        }
    }
}
