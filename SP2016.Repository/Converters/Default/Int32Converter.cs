using System;
using System.Globalization;

namespace SP2016.Repository.Converters.Default
{
    public class Int32Converter : BaseConverter
    {
        public override object ConvertPropertyValueToFieldValue(object propertyValue)
        {
            return ((int)propertyValue).ToString(CultureInfo.InvariantCulture);
        }

        public override object ConvertFieldValueToPropertyValue(object fieldValue)
        {
            return Convert.ToInt32(fieldValue, CultureInfo.InvariantCulture);
        }
    }
}
