using System;

namespace SP2016.Repository.Attributes
{
    public class FieldMappingAttribute : Attribute
    {
        public bool IsReadOnly { get; }

        public string FieldName { get; }

        public string FieldValuesConverterTypeName { get; set; }

        public FieldMappingAttribute(string fieldName)
        {
            FieldName = fieldName;
        }

        public FieldMappingAttribute(string fieldName, bool isReadOnly) : this(fieldName)
        {
            IsReadOnly = isReadOnly;
        }

        public FieldMappingAttribute(string fieldName, bool isReadOnly, Type fieldValuesConverterType) : this(fieldName, isReadOnly)
        {
            FieldValuesConverterTypeName = fieldValuesConverterType.AssemblyQualifiedName;
        }

        public FieldMappingAttribute(string fieldName, Type fieldValuesConverterType) : this(fieldName)
        {
            FieldValuesConverterTypeName = fieldValuesConverterType.AssemblyQualifiedName;
        }
    }
}
