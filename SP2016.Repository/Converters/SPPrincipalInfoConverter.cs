using Microsoft.SharePoint;
using SP2016.Repository.Models;

namespace SP2016.Repository.Converters
{
    public class SPPrincipalInfoConverter : BaseSharePointConverter
    {
        public override object ConvertFieldValueToPropertyValue(object fieldValue)
        {
            var userValue = new SPFieldUserValue(Web, fieldValue.ToString());
            var principal = userValue.User ?? (SPPrincipal)Web.SiteGroups.GetByID(userValue.LookupId);
            return new SPPrincipalInfo
            {
                ID = principal.ID,
                Name = principal.Name,
                UrlMask = (principal is SPGroup) ? "/_layouts/people.aspx?MembershipGroupId=" : "/_layouts/userdisp.aspx?ID=",
                IsUser = principal is SPUser
            };
        }

        public override object ConvertPropertyValueToFieldValue(object propertyValue)
        {
            var principal = (SPPrincipalInfo)propertyValue;
            return new SPFieldUserValue(Web, principal.ID, principal.Name);
        }
    }
}
