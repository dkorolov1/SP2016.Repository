using Microsoft.SharePoint;

namespace SP2016.Repository.Models
{
    /// <summary>
    /// Сериализуемый аналог класса SPPrincipal
    /// </summary>
    public class SPPrincipalInfo
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string UrlMask { get; set; }
        public string Url => UrlMask + ID;
        public bool IsUser { get; set; }

        public SPPrincipalInfo() { }

        public SPPrincipalInfo(SPWeb web, int? id = null)
        {
            if (!id.HasValue)
                id = web.CurrentUser.ID;

            var userValue = new SPFieldUserValue(web, id.Value, "");
            if (userValue.User == null)
            {
                var group = web.SiteGroups.GetByID(userValue.LookupId);
                FillPropertiesFrom(group);
            }
            else
                FillPropertiesFrom(userValue.User);

        }

        public SPPrincipalInfo(SPWeb web, SPFieldUserValue userValue) 
            : this(web, userValue.LookupId) { }

        private void FillPropertiesFrom(SPUser user)
        {
            ID = user.ID;
            Name = user.Name;
            UrlMask = "/_layouts/userdisp.aspx?ID=";
            IsUser = true;
        }

        private void FillPropertiesFrom(SPGroup group)
        {
            ID = group.ID;
            Name = group.Name;
            UrlMask = "/_layouts/people.aspx?MembershipGroupId=";
            IsUser = true;
        }

        public SPPrincipalInfo(SPUser user)
        {
            FillPropertiesFrom(user);
        }

        public SPPrincipalInfo(SPGroup group)
        {
            FillPropertiesFrom(group);
        }

        public SPUser GetUser(SPWeb web)
        {
            return web.SiteUsers.GetByID(ID);
        }

        public SPGroup GetGroup(SPWeb web)
        {
            return web.SiteGroups.GetByID(ID);
        }

        public SPFieldUserValue GetUserValue(SPWeb web)
        {
            return new SPFieldUserValue(web, ID, Name);
        }
    }
}
