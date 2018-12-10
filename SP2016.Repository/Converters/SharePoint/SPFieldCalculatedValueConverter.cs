using Microsoft.SharePoint;
using System.Reflection;

namespace SP2016.Repository.Converters.SharePoint
{
    public class SPFieldCalculatedValueConverter : SharePointConverter
    {
        public override object ConvertFieldValueToPropertyValue(SPWeb web, SPField field, PropertyInfo propertyInfo, object fieldValue)
        {
            return new SPFieldCalculatedValue((field as SPFieldCalculated).TypeAsString, field.GetFieldValueAsText(fieldValue));
        }

        public override object ConvertPropertyValueToFieldValue(SPWeb web, SPField field, PropertyInfo propertyInfo, object propertyValue)
        {
            return (propertyValue as SPFieldCalculatedValue).ToString();
        }
    }
}
