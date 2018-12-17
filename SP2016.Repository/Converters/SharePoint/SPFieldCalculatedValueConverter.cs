using Microsoft.SharePoint;
using System.Reflection;

namespace SP2016.Repository.Converters.SharePoint
{
    public class SPFieldCalculatedValueConverter : SPFieldConverter
    {
        public override object ConvertFieldValueToPropertyValue(PropertyInfo propertyInfo, object fieldValue)
        {
            return new SPFieldCalculatedValue((Field as SPFieldCalculated).TypeAsString, Field.GetFieldValueAsText(fieldValue));
        }

        public override object ConvertPropertyValueToFieldValue(PropertyInfo propertyInfo, object propertyValue)
        {
            return (propertyValue as SPFieldCalculatedValue).ToString();
        }
    }
}
