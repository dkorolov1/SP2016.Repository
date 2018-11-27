using System;

namespace SP2016.Repository.Converters
{
    public class AfterPropertiesStringValueConverter : BaseSharePointConverter
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