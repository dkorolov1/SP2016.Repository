using System;

namespace SP2016.Repository.Converters.Default
{
    public class StringValueConverter : BaseConverter
    {
        public override object ConvertFieldValueToPropertyValue(object fieldValue)
        {
            return Convert.ToString(fieldValue);
        }

        public override object ConvertPropertyValueToFieldValue(object propertyValue)
        {
            return Convert.ToString(propertyValue);
        }
    }
}
