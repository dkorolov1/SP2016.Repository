using Microsoft.SharePoint;
using System;
using System.Reflection;

namespace SP2016.Repository.Converters.SharePoint
{
    public class SPFieldLookupConverter : SPFieldConverter
    {
        public override object ConvertPropertyValueToFieldValue(PropertyInfo propertyInfo, object propertyValue)
        {
            // Обработка ошибки,возникающей в случае, когда значение типа SPFieldLookupValue имеет LookupId = 0                    
            if (propertyValue is SPFieldLookupValue)
            {
                SPFieldLookupValue lookupValue = (SPFieldLookupValue)propertyValue;
                if (lookupValue.LookupId == 0)
                    return null;
            }

            return propertyValue;
        }

        public override object ConvertFieldValueToPropertyValue(PropertyInfo propertyInfo, object fieldValue)
        {
            string valueStr = Convert.ToString(fieldValue);
            if (propertyInfo.PropertyType.Equals(typeof(string)))
                return valueStr;

            if ((Field as SPFieldLookup).AllowMultipleValues)
                return new SPFieldLookupValueCollection(valueStr);
            else
                return new SPFieldLookupValue(valueStr);
        }
    }
}
