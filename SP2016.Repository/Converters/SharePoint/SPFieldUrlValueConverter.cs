using Microsoft.SharePoint;
using System;
using System.Reflection;

namespace SP2016.Repository.Converters.SharePoint
{
    public class SPFieldUrlValueConverter : SPFieldConverter
    {
        public override object ConvertFieldValueToPropertyValue(PropertyInfo propertyInfo, object fieldValue)
        {
            return fieldValue == null ? null
                : new SPFieldUrlValue(Convert.ToString(fieldValue));
        }
    }
}
