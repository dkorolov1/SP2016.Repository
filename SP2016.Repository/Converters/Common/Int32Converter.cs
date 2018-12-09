using System;
using System.Globalization;
using System.Reflection;

namespace SP2016.Repository.Converters.Common
{
    public class Int32Converter : SimpleConverter
    {
        public override object ConvertPropertyValueToFieldValue(PropertyInfo propertyInfo, object propertyValue)
        {
            return ((int)propertyValue).ToString(CultureInfo.InvariantCulture);
        }

        public override object ConvertFieldValueToPropertyValue(PropertyInfo propertyInfo, object fieldValue)
        {
            return Convert.ToInt32(fieldValue, CultureInfo.InvariantCulture);
        }
    }
}
