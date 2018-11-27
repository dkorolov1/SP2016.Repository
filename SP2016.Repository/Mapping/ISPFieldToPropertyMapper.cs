using Microsoft.SharePoint;
using SP2016.Repository.Mapping;
using SP2016.Repository.Converters.Default;
using System.Reflection;

namespace SP2016.Repository.Mapping
{
    public interface ISPFieldToPropertyMapper : IListItemFieldMapper
    {
        BaseConverter GetDefaultConverter(SPWeb web, PropertyInfo propertyInfo, SPField field);

        BaseConverter GetBatchFieldValuesConverter(SPWeb web, PropertyInfo propertyInfo, SPField field);

        BaseConverter GetAfterPropertiesFieldValuesConverter(SPWeb web, PropertyInfo propertyInfo, SPField field);

        BaseConverter GetItemVersionFieldValuesConverter(SPWeb web, PropertyInfo propertyInfo, SPField field);
    }
}
