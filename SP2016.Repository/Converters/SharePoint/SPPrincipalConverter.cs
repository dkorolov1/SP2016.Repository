using System.Reflection;
using Microsoft.SharePoint;

namespace SP2016.Repository.Converters.SharePoint
{
    public class SPPrincipalConverter : SharePointConverter
    {
        public override object ConvertFieldValueToPropertyValue(SPWeb web, SPField field, PropertyInfo propertyInfo, object fieldValue)
        {
            var userValue = new SPFieldUserValue(web, fieldValue.ToString());
            var principal = userValue.User ?? (SPPrincipal)web.SiteGroups.GetByID(userValue.LookupId);
            return principal;
        }

        public override object ConvertPropertyValueToFieldValue(SPWeb web, SPField field, PropertyInfo propertyInfo, object propertyValue)
        {
            var principal = (SPPrincipal)propertyValue;
            return new SPFieldUserValue(web, principal.ID, principal.Name);
        }
    }
}
