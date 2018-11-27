using Microsoft.SharePoint;

namespace SP2016.Repository.Converters
{
    public class SPFieldUrlValueConverter : BaseSharePointConverter
    {
        public override object ConvertFieldValueToPropertyValue(object fieldValue)
        {
            return new SPFieldUrlValue(fieldValue.ToString());
        }
    }
}
