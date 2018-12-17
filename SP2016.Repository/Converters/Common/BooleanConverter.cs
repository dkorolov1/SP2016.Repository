using System.Reflection;

namespace SP2016.Repository.Converters.Common
{
    public class BooleanConverter : FieldConverter
    {
        public override object ConvertPropertyValueToFieldValue(PropertyInfo propertyInfo, object propertyValue)
        {
            return propertyValue;
        }

        public override object ConvertFieldValueToPropertyValue(PropertyInfo propertyInfo, object fieldValue)
        {
            string str = fieldValue.ToString();
            switch (str.ToLower())
            {
                //mvc checkbox
                case "true,false":
                case "yes":
                case "да":
                case "1":
                case "true":
                case "on":
                    return true;
                default:
                    return false;
            }
        }
    }
}
