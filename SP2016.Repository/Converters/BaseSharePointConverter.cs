using Microsoft.SharePoint;
using SP2016.Repository.Converters.Default;

namespace SP2016.Repository.Converters
{
    public class BaseSharePointConverter : BaseConverter
    {
        public SPField FieldInfo { get; set; }

        public SPWeb Web { get; set; }
    }
}
