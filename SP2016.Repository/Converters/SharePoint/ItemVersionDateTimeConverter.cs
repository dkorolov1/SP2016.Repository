using Microsoft.SharePoint;
using System;
using System.Reflection;

namespace SP2016.Repository.Converters.SharePoint
{
    public class ItemVersionDateTimeConverter : SPFieldConverter
    {
        public override object ConvertFieldValueToPropertyValue(PropertyInfo propertyInfo, object fieldValue)
        {
            //Время в версионном представлении храниться в Гринвиче
            SPTimeZone timeZone = Web.RegionalSettings.TimeZone;

            // конвертирование даты и времени в локальные:
            return timeZone.UTCToLocalTime((DateTime)fieldValue);
        }
    }
}
