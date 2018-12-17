using System.Reflection;
using Microsoft.SharePoint;

namespace SP2016.Repository.Converters.SharePoint
{
    public class SPPrincipalConverter : SPFieldConverter
    {
        public override object ConvertFieldValueToPropertyValue(PropertyInfo propertyInfo, object fieldValue)
        {
            var userValue = new SPFieldUserValue(Web, fieldValue.ToString());
            var principal = userValue.User ?? (SPPrincipal)Web.SiteGroups.GetByID(userValue.LookupId);
            return principal;
        }

        public override object ConvertPropertyValueToFieldValue(PropertyInfo propertyInfo, object propertyValue)
        {
            var principal = (SPPrincipal)propertyValue;
            return new SPFieldUserValue(Web, principal.ID, principal.Name);
        }
    }
}
