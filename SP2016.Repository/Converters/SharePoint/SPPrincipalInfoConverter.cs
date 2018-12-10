using Microsoft.SharePoint;
using SP2016.Repository.Models;
using System.Reflection;

namespace SP2016.Repository.Converters.SharePoint
{
    public class SPPrincipalInfoConverter : SharePointConverter
    {
        public override object ConvertFieldValueToPropertyValue(SPWeb web, SPField field, PropertyInfo propertyInfo, object fieldValue)
        {
            var userValue = new SPFieldUserValue(web, fieldValue.ToString());
            var principal = userValue.User ?? (SPPrincipal)web.SiteGroups.GetByID(userValue.LookupId);
            return new SPPrincipalInfo
            {
                ID = principal.ID,
                Name = principal.Name,
                UrlMask = (principal is SPGroup) ? "/_layouts/people.aspx?MembershipGroupId=" : "/_layouts/userdisp.aspx?ID=",
                IsUser = principal is SPUser
            };
        }

        public override object ConvertPropertyValueToFieldValue(SPWeb web, SPField field, PropertyInfo propertyInfo, object propertyValue)
        {
            var principal = (SPPrincipalInfo)propertyValue;
            return new SPFieldUserValue(web, principal.ID, principal.Name);
        }
    }
}
