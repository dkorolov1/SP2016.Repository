using Microsoft.SharePoint;
using SP2016.Repository.Converters.Default;
using System.Reflection;

namespace SP2016.Repository.Mapping
{
    public interface ISPFieldToPropertyMapper
    {
        SimpleConverter GetDefaultConverter(SPWeb web, PropertyInfo propertyInfo, SPField field);

        SimpleConverter GetBatchFieldValuesConverter(SPWeb web, PropertyInfo propertyInfo, SPField field);

        SimpleConverter GetAfterPropertiesFieldValuesConverter(SPWeb web, PropertyInfo propertyInfo, SPField field);

        SimpleConverter GetItemVersionFieldValuesConverter(SPWeb web, PropertyInfo propertyInfo, SPField field);
    }
}
