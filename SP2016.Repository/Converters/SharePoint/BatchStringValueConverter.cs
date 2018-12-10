using Microsoft.SharePoint;
using System.Reflection;
using System;

namespace SP2016.Repository.Converters.SharePoint
{
    public class BatchStringValueConverter : SharePointConverter
    {
        public override object ConvertFieldValueToPropertyValue(SPWeb web, SPField field, PropertyInfo propertyInfo, object fieldValue)
        {
            return field.GetFieldValueAsText(fieldValue);
        }

        public override object ConvertPropertyValueToFieldValue(SPWeb web, SPField field, PropertyInfo propertyInfo, object propertyValue)
        {
            string value = Convert.ToString(propertyValue);

            if (field.Type != SPFieldType.Note)
                return value;

            return $"<![CDATA[{value}]]>";
        }
    }
}
