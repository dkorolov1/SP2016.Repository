using System;
using System.Globalization;
using System.Reflection;

namespace SP2016.Repository.Converters.Common
{
    public class SingleConverter : SimpleConverter
    {
        public override object ConvertPropertyValueToFieldValue(PropertyInfo propertyInfo, object propertyValue)
        {
            return ((float)propertyValue).ToString(CultureInfo.InvariantCulture);
        }

        public override object ConvertFieldValueToPropertyValue(PropertyInfo propertyInfo, object fieldValue)
        {
            return Convert.ToSingle(fieldValue, CultureInfo.InvariantCulture);
        }
    }
}
