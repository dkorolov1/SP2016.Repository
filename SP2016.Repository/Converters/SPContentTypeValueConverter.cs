namespace SP2016.Repository.Converters
{
    class SPContentTypeValueConverter : BaseSharePointConverter
    {
        public override object ConvertFieldValueToPropertyValue(object fieldValue)
        {
            return Web.ContentTypes[fieldValue.ToString()];
        }
    }
}
