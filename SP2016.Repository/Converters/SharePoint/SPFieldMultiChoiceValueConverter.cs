using Microsoft.SharePoint;
using System.Reflection;

namespace SP2016.Repository.Converters.SharePoint
{
    public class SPFieldMultiChoiceValueConverter : SPFieldConverter
    {
        public override object ConvertFieldValueToPropertyValue(PropertyInfo propertyInfo, object fieldValue)
        {
            return new SPFieldMultiChoiceValue(fieldValue.ToString());
        }
    }
}
