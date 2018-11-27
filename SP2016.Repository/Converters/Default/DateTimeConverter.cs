using System;

namespace SP2016.Repository.Converters.Default
{
    public class DateTimeConverter : BaseConverter
    {
        public override object ConvertFieldValueToPropertyValue(object fieldValue)
        {
            if (fieldValue == null) return null;
            if (string.IsNullOrWhiteSpace(fieldValue?.ToString()))
                return null;

            if (fieldValue.GetType() == typeof(DateTime))
                return fieldValue;

            return DateTime.Parse(fieldValue.ToString());
        }

        public override object ConvertPropertyValueToFieldValue(object propertyValue)
        {
            return propertyValue;
        }
    }
}
