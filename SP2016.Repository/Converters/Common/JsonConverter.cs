using Newtonsoft.Json;
using System;
using System.Reflection;

namespace SP2016.Repository.Converters.Common
{
    public class JsonConverter : FieldConverter
    {
        public override object ConvertPropertyValueToFieldValue(PropertyInfo propertyInfo, object propertyValue)
        {
            return JsonConvert.SerializeObject(propertyValue);
        }

        public override object ConvertFieldValueToPropertyValue(PropertyInfo propertyInfo, object fieldValue)
        {
            var jsonString = fieldValue as string;

            if (jsonString == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject(jsonString, propertyInfo.PropertyType);
        }
    }
}
