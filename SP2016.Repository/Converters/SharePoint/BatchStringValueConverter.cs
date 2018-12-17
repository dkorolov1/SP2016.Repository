using Microsoft.SharePoint;
using System.Reflection;
using System;

namespace SP2016.Repository.Converters.SharePoint
{
    public class BatchStringValueConverter : SPFieldConverter
    {
        public override object ConvertFieldValueToPropertyValue(PropertyInfo propertyInfo, object fieldValue)
        {
            return Field.GetFieldValueAsText(fieldValue);
        }

        public override object ConvertPropertyValueToFieldValue(PropertyInfo propertyInfo, object propertyValue)
        {
            string value = Convert.ToString(propertyValue);

            if (Field.Type != SPFieldType.Note)
                return value;

            return $"<![CDATA[{value}]]>";
        }
    }
}
