using Microsoft.SharePoint;
using System.Reflection;

namespace SP2016.Repository.Converters.SharePoint
{
    public class SharePointConverter : IConverter
    {
        public virtual object ConvertPropertyValueToFieldValue(SPWeb web, SPField field, PropertyInfo propertyInfo, object propertyValue)
        {
            return propertyValue;
        }

        public virtual object ConvertFieldValueToPropertyValue(SPWeb web, SPField field, PropertyInfo propertyInfo, object fieldValue)
        {
            return fieldValue;
        }
    }
}
