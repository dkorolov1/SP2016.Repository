using Microsoft.SharePoint;
using System.Reflection;

namespace SP2016.Repository.Converters.SharePoint
{
    public class SPFieldUserConverter : SPFieldLookupConverter
    {
        public override object ConvertPropertyValueToFieldValue(SPWeb web, SPField field, PropertyInfo propertyInfo, object propertyValue)
        {
            if (!(field as SPFieldUser).AllowMultipleValues)
                return base.ConvertPropertyValueToFieldValue(web, field, propertyInfo, propertyValue);
            else
                return propertyValue;
        }

        public override object ConvertFieldValueToPropertyValue(SPWeb web, SPField field, PropertyInfo propertyInfo, object fieldValue)
        {
            if ((field as SPFieldUser).AllowMultipleValues)
                return new SPFieldUserValueCollection(web, fieldValue.ToString());
            else
                return new SPFieldUserValue(web, fieldValue.ToString());
        }
    }
}
