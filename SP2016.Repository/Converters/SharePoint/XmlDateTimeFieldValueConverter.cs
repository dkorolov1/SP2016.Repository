using Microsoft.SharePoint.Utilities;
using System;
using System.Reflection;

namespace SP2016.Repository.Converters.SharePoint
{
    public class XmlDateTimeFieldValueConverter : SPFieldConverter
    {
        public override object ConvertFieldValueToPropertyValue(PropertyInfo propertyInfo, object fieldValue)
        {
            DateTime value = DateTime.ParseExact(fieldValue.ToString(), "yyyy-MM-ddTHH:mm:ssZ", null).ToUniversalTime();
            return value;
        }

        public override object ConvertPropertyValueToFieldValue(PropertyInfo propertyInfo, object propertyValue)
        {
            if (propertyValue is DateTime value)
            {
                return SPUtility.CreateISO8601DateTimeFromSystemDateTime(value);
            }
            else
                return null;
        }
    }
}
