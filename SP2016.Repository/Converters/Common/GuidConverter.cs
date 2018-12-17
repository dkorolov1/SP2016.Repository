using System;
using System.Reflection;

namespace SP2016.Repository.Converters.Common
{
    public class GuidConverter : FieldConverter
    {
        public override object ConvertPropertyValueToFieldValue(PropertyInfo propertyInfo, object propertyValue)
        {
            if ((Guid)(propertyValue) == Guid.Empty)
                return String.Empty;
            else
                return propertyValue.ToString();
        }

        public override object ConvertFieldValueToPropertyValue(PropertyInfo propertyInfo, object fieldValue)
        {
            return new Guid(fieldValue.ToString());
        }
    }
}
