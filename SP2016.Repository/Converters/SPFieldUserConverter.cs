using Microsoft.SharePoint;

namespace SP2016.Repository.Converters
{
    public class SPFieldUserConverter : SPFieldLookupConverter
    {
        public override object ConvertPropertyValueToFieldValue(object propertyValue)
        {
            if (!(FieldInfo as SPFieldUser).AllowMultipleValues)
                return base.ConvertPropertyValueToFieldValue(propertyValue);
            else
                return propertyValue;
        }

        public override object ConvertFieldValueToPropertyValue(object fieldValue)
        {
            if ((FieldInfo as SPFieldUser).AllowMultipleValues)
                return new SPFieldUserValueCollection(Web, fieldValue.ToString());
            else
                return new SPFieldUserValue(Web, fieldValue.ToString());
        }
    }
}
