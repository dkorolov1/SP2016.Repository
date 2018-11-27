using Microsoft.SharePoint;

namespace SP2016.Repository.Converters
{
    public class SPFieldUserValueCollectionConverter : SPFieldUserConverter
    {
        public override object ConvertPropertyValueToFieldValue(object propertyValue)
        {
            if (!(FieldInfo as SPFieldUser).AllowMultipleValues)
                return base.ConvertPropertyValueToFieldValue(propertyValue);
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
