using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.Reflection;

namespace SP2016.Repository.Converters.SharePoint
{
    public class XmlDateTimeFieldValueConverter : SharePointConverter
    {
        public override object ConvertFieldValueToPropertyValue(SPWeb web, SPField field, PropertyInfo propertyInfo, object fieldValue)
        {
            DateTime value = DateTime.ParseExact(fieldValue.ToString(), "yyyy-MM-ddTHH:mm:ssZ", null).ToUniversalTime();
            return value;
        }

        public override object ConvertPropertyValueToFieldValue(SPWeb web, SPField field, PropertyInfo propertyInfo, object propertyValue)
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
