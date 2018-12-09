using System;
using System.Reflection;

namespace SP2016.Repository.Converters.Common
{
    public class DateTimeConverter : SimpleConverter
    {
        public override object ConvertFieldValueToPropertyValue(PropertyInfo propertyInfo, object fieldValue)
        {
            if (fieldValue == null) return null;
            if (string.IsNullOrWhiteSpace(fieldValue?.ToString()))
                return null;

            if (fieldValue.GetType() == typeof(DateTime))
                return fieldValue;

            return DateTime.Parse(fieldValue.ToString());
        }

        public override object ConvertPropertyValueToFieldValue(PropertyInfo propertyInfo, object propertyValue)
        {
            return propertyValue;
        }
    }
}
