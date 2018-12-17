using Microsoft.SharePoint;
using System.Reflection;

namespace SP2016.Repository.Converters.SharePoint
{
    public class SPFieldUserValueCollectionConverter : SPFieldUserConverter
    {
        public override object ConvertPropertyValueToFieldValue(PropertyInfo propertyInfo, object propertyValue)
        {
            if (!(Field as SPFieldUser).AllowMultipleValues)
                return base.ConvertPropertyValueToFieldValue(propertyInfo, propertyValue);
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
