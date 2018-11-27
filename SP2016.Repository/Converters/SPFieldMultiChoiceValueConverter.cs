using Microsoft.SharePoint;

namespace SP2016.Repository.Converters
{
    public class SPFieldMultiChoiceValueConverter : BaseSharePointConverter
    {
        public override object ConvertFieldValueToPropertyValue(object fieldValue)
        {
            return new SPFieldMultiChoiceValue(fieldValue.ToString());
        }
    }
}
