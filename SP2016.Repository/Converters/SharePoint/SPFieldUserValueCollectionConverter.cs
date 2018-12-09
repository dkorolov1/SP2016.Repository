using Microsoft.SharePoint;
using System.Reflection;

namespace SP2016.Repository.Converters.SharePoint
{
    public class SPFieldUserValueCollectionConverter : SPFieldUserConverter
    {
        public override object ConvertPropertyValueToFieldValue(SPWeb web, SPField field, PropertyInfo propertyInfo, object propertyValue)
        {
            if (!(field as SPFieldUser).AllowMultipleValues)
                return base.ConvertPropertyValueToFieldValue(web, field, propertyInfo, propertyValue);
            else
            {
                if (propertyValue != null)
                    return propertyValue.ToString();
                else
                    return null;
            }
        }
    }
}
