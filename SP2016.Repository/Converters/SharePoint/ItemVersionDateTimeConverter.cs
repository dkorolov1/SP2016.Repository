using Microsoft.SharePoint;
using System;
using System.Reflection;

namespace SP2016.Repository.Converters.SharePoint
{
    public class ItemVersionDateTimeConverter : SharePointConverter
    {
        public override object ConvertFieldValueToPropertyValue(SPWeb web, SPField field, PropertyInfo propertyInfo, object fieldValue)
        {
            //Время в версионном представлении храниться в Гринвиче
            SPTimeZone timeZone = web.RegionalSettings.TimeZone;

            // конвертирование даты и времени в локальные:
            return timeZone.UTCToLocalTime((DateTime)fieldValue);
        }
    }
}
