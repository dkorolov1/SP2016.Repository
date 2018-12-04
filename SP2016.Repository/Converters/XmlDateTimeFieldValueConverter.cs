using Microsoft.SharePoint.Utilities;
using System;

namespace SP2016.Repository.Converters
{
    public class XmlDateTimeFieldValueConverter : BaseSharePointConverter
    {
        public override object ConvertFieldValueToPropertyValue(object fieldValue)
        {
            DateTime value = DateTime.ParseExact(fieldValue.ToString(), "yyyy-MM-ddTHH:mm:ssZ", null).ToUniversalTime();
            return value;
        }

        public override object ConvertPropertyValueToFieldValue(object propertyValue)
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
