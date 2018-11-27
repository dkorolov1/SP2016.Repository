namespace SP2016.Repository.Converters.Default
{
    public class BooleanConverter : BaseConverter
    {
        public override object ConvertPropertyValueToFieldValue(object propertyValue)
        {
            return propertyValue;
        }

        public override object ConvertFieldValueToPropertyValue(object fieldValue)
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
