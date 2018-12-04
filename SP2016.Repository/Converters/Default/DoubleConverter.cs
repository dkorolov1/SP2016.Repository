using System;
using System.Globalization;

namespace SP2016.Repository.Converters.Default
{
    public class DoubleConverter : SimpleConverter
    {
        public override object ConvertPropertyValueToFieldValue(object propertyValue)
        {
            return ((double)propertyValue).ToString(CultureInfo.InvariantCulture);
        }

        public override object ConvertFieldValueToPropertyValue(object fieldValue)
        {
            if (fieldValue == null || string.IsNullOrWhiteSpace(fieldValue.ToString()))
                return null;

            return Convert.ToDouble(fieldValue, CultureInfo.InvariantCulture);
        }
    }
}
