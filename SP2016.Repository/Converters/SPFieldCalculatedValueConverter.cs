using Microsoft.SharePoint;
using SP2016.Repository;

namespace SP2016.Repository.Converters
{
    public class SPFieldCalculatedValueConverter : BaseSharePointConverter
    {
        public override object ConvertFieldValueToPropertyValue(object fieldValue)
        {
            return new SPFieldCalculatedValue((FieldInfo as SPFieldCalculated).TypeAsString, FieldInfo.GetFieldValueAsText(fieldValue));
        }

        public override object ConvertPropertyValueToFieldValue(object propertyValue)
        {
            return (propertyValue as SPFieldCalculatedValue).ToString();
        }
    }
}
