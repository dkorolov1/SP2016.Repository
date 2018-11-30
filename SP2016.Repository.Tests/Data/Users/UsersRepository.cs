using Microsoft.SharePoint;
using SP2016.Repository.Caml;
using SP2016.Repository.Mapping;

namespace SP2016.Repository.Tests
{
    public class UsersRepository : BaseEntityRepository<UserEntity>
    {
        public override string ListName => "Users";

        protected override FieldToPropertyMapping[] FieldMappings => new FieldToPropertyMapping[]
        {
            new FieldToPropertyMapping("Title", "DisplayName"),
            new FieldToPropertyMapping("Dismissed"),
            new FieldToPropertyMapping("Description"),
            new FieldToPropertyMapping("BirthDate"),
            new FieldToPropertyMapping("Department"),
            new FieldToPropertyMapping("Link"),
            new FieldToPropertyMapping("JobTitle"),
            new FieldToPropertyMapping("Seniority")
        };

        public UserEntity[] GetByJobTitle(SPWeb web, string jobTitle, uint rowLimit = 0)
        {
            var filter1 = new Filter(Enums.FilterType.Contains, "JobTitle", jobTitle, FilterValueType.Text);
            var query = new Query(filter1);

            return GetEntities(web, filter1, rowLimit);
        }
    }
}