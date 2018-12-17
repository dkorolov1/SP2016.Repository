using System;
using System.Reflection;

namespace SP2016.Repository.Converters.Common
{
    public class StringValueConverter : FieldConverter
    {
        public override object ConvertFieldValueToPropertyValue(PropertyInfo propertyInfo, object fieldValue)
        {
            return Convert.ToString(fieldValue);
        }

        public override object ConvertPropertyValueToFieldValue(PropertyInfo propertyInfo, object propertyValue)
        {
            return Convert.ToString(propertyValue);
        }
    }
}
