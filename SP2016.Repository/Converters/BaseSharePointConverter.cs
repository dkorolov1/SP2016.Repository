using Microsoft.SharePoint;
using System.Reflection;

namespace SP2016.Repository.Converters
{
    public class SharePointConverter : IConverter
    {
        public virtual object ConvertPropertyValueToFieldValue(SPWeb web, PropertyInfo PropertyInfo, object propertyValue)
        {
            return propertyValue;
        }

        public virtual object ConvertFieldValueToPropertyValue(SPWeb web, PropertyInfo PropertyInfo, object fieldValue)
        {
            return fieldValue;
        }
    }
}
