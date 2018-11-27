using Microsoft.SharePoint;

namespace SP2016.Repository.Converters
{
    public class SPPrincipalConverter : BaseSharePointConverter
    {
        public override object ConvertFieldValueToPropertyValue(object fieldValue)
        {
            var userValue = new SPFieldUserValue(Web, fieldValue.ToString());
            var principal = userValue.User ?? (SPPrincipal)Web.SiteGroups.GetByID(userValue.LookupId);
            return principal;
        }

        public override object ConvertPropertyValueToFieldValue(object propertyValue)
        {
            var principal = (SPPrincipal)propertyValue;
            return new SPFieldUserValue(Web, principal.ID, principal.Name);
        }
    }
}
