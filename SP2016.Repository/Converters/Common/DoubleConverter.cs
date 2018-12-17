using System;
using System.Globalization;
using System.Reflection;

namespace SP2016.Repository.Converters.Common
{
    public class DoubleConverter : FieldConverter
    {
        public override object ConvertPropertyValueToFieldValue(PropertyInfo propertyInfo, object propertyValue)
        {
            return ((double)propertyValue).ToString(CultureInfo.InvariantCulture);
        }

        public override object ConvertFieldValueToPropertyValue(PropertyInfo propertyInfo, object fieldValue)
        {
            if (fieldValue == null || string.IsNullOrWhiteSpace(fieldValue.ToString()))
                return null;

            return Convert.ToDouble(fieldValue, CultureInfo.InvariantCulture);
        }
    }
}
