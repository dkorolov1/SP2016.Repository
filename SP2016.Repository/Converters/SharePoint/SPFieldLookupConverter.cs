using Microsoft.SharePoint;
using System.Reflection;

namespace SP2016.Repository.Converters.SharePoint
{
    public class SPFieldLookupConverter : SharePointConverter
    {
        public override object ConvertPropertyValueToFieldValue(SPWeb web, SPField field, PropertyInfo propertyInfo, object propertyValue)
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
        public override object ConvertFieldValueToPropertyValue(SPWeb web, SPField field, PropertyInfo propertyInfo, object fieldValue)
        {
            string valueStr = fieldValue.ToString();
            if (propertyInfo.PropertyType.Equals(typeof(string)))
                return valueStr;
            else if ((field as SPFieldLookup).AllowMultipleValues)
                return new SPFieldLookupValueCollection(valueStr);
            else
                return new SPFieldLookupValue(valueStr);
        }
    }
}
