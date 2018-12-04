using System;
using System.Globalization;

namespace SP2016.Repository.Converters.Default
{
    public class SingleConverter : SimpleConverter
    {
        public override object ConvertPropertyValueToFieldValue(object propertyValue)
        {
            return ((float)propertyValue).ToString(CultureInfo.InvariantCulture);
        }

        public override object ConvertFieldValueToPropertyValue(object fieldValue)
        {
            return Convert.ToSingle(fieldValue, CultureInfo.InvariantCulture);
        }
    }
}
