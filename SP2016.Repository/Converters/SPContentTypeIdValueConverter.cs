using Microsoft.SharePoint;

namespace SP2016.Repository.Converters
{
    public class SPContentTypeIdValueConverter : BaseSharePointConverter
    {
        public override object ConvertFieldValueToPropertyValue(object fieldValue)
        {
            return new SPContentTypeId(fieldValue.ToString());
        }
    }
}
