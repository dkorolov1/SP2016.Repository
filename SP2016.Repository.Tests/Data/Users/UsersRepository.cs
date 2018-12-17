using Microsoft.SharePoint;
using SP2016.Repository.Caml;
using SP2016.Repository.Mapping;

namespace SP2016.Repository.Tests
{
    public class UsersRepository : SharePointRepository<UserEntity>
    {
        public override string ListName => "Users";

        public UserEntity[] GetByJobTitle(SPWeb web, string jobTitle, uint rowLimit = 0)
        {
            var filter1 = new Filter(Enums.FilterType.Contains, "JobTitle", jobTitle, FilterValueType.Text);
            var query = new Query(filter1);

            return GetEntities(web, filter1, true, rowLimit);
        }
    }
}