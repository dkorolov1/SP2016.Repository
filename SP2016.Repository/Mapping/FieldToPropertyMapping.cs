using SP2016.Repository.Converters;
using System.Diagnostics;
using System.Reflection;

namespace SP2016.Repository.Mapping
{
    [DebuggerDisplay("F:{FieldName}, P:{EntityPropertyName}")]
    public class FieldToPropertyMapping
    {
        public string FieldName { get; }

        public PropertyInfo PropertyInfo { get; }

        public bool ReadOnly { get; }

        public FieldConverter Converter { get; }

        public FieldToPropertyMapping(string fieldName, PropertyInfo property, FieldConverter converter, bool readOnly)
        {
            FieldName = fieldName;
            PropertyInfo = property;
            ReadOnly = readOnly;
            Converter = converter;
        }
    }
}
