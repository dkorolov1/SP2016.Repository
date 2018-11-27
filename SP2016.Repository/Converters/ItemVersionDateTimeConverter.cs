using Microsoft.SharePoint;
using System;

namespace SP2016.Repository.Converters
{
    public class ItemVersionDateTimeConverter : BaseSharePointConverter
    {
        public override object ConvertFieldValueToPropertyValue(object fieldValue)
        {
            //Время в версионном представлении храниться в Гринвиче
            SPTimeZone timeZone = Web.RegionalSettings.TimeZone;
            // конвертирование даты и времени в локальные:
            return timeZone.UTCToLocalTime((DateTime)fieldValue);
        }
    }
}
