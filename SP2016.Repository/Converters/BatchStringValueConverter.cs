using Microsoft.SharePoint;
using System;

namespace SP2016.Repository.Converters
{
    public class BatchStringValueConverter : BaseSharePointConverter
    {
        public override object ConvertFieldValueToPropertyValue(object fieldValue)
        {
            return FieldInfo.GetFieldValueAsText(fieldValue);
        }

        public override object ConvertPropertyValueToFieldValue(object propertyValue)
        {
            string value = Convert.ToString(propertyValue);

            if (FieldInfo.Type != SPFieldType.Note)
                return value;

            return $"<![CDATA[{value}]]>";
        }
    }
}
