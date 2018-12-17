using Microsoft.SharePoint;
using System.Reflection;

namespace SP2016.Repository.Converters.SharePoint
{
    public class SPFieldUserConverter : SPFieldLookupConverter
    {
        public override object ConvertPropertyValueToFieldValue(PropertyInfo propertyInfo, object propertyValue)
        {
            if (!(Field as SPFieldUser).AllowMultipleValues)
                return base.ConvertPropertyValueToFieldValue(propertyInfo, propertyValue);
            else
                return propertyValue;
        }

        public override object ConvertFieldValueToPropertyValue(PropertyInfo propertyInfo, object fieldValue)
        {
            if ((Field as SPFieldUser).AllowMultipleValues)
                return new SPFieldUserValueCollection(Web, fieldValue.ToString());
            else
                return new SPFieldUserValue(Web, fieldValue.ToString());
        }
    }
}
