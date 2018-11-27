using Microsoft.SharePoint;

namespace SP2016.Repository.Converters
{
    public class SPFieldLookupConverter : BaseSharePointConverter
    {
        public override object ConvertPropertyValueToFieldValue(object propertyValue)
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
        public override object ConvertFieldValueToPropertyValue(object fieldValue)
        {
            string valueStr = fieldValue.ToString();
            if (PropertyInfo.PropertyType.Equals(typeof(string)))
                return valueStr;
            else if ((FieldInfo as SPFieldLookup).AllowMultipleValues)
                return new SPFieldLookupValueCollection(valueStr);
            else
                return new SPFieldLookupValue(valueStr);
        }
    }
}
