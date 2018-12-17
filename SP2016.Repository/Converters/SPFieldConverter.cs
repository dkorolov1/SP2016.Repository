using Microsoft.SharePoint;

namespace SP2016.Repository.Converters
{
    public class SPFieldConverter : FieldConverter
    {
        public SPWeb Web { get; set; }

        public SPField Field { get; set; }
    }
}