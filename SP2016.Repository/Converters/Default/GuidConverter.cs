using System;

namespace SP2016.Repository.Converters.Default
{
    public class GuidConverter : BaseConverter
    {
        public override object ConvertPropertyValueToFieldValue(object propertyValue)
        {
            if ((Guid)(propertyValue) == Guid.Empty)
                return String.Empty;
            else
                return propertyValue.ToString();
        }

        public override object ConvertFieldValueToPropertyValue(object fieldValue)
        {
            return new Guid(fieldValue.ToString());
        }
    }
}
