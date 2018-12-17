using Microsoft.SharePoint;
using System.Reflection;

namespace SP2016.Repository.Converters.SharePoint
{
    public class SPContentTypeIdValueConverter : SPFieldConverter
    {
        public override object ConvertFieldValueToPropertyValue(PropertyInfo propertyInfo, object fieldValue)
        {
            return new SPContentTypeId(fieldValue.ToString());
        }
    }
}
