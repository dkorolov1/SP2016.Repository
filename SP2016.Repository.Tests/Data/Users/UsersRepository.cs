using SP2016.Repository.Mapping;

namespace SP2016.Repository.Tests
{
    public class UsersRepository : BaseEntityRepository<UserEntity>
    {
        public override string ListName => "Users";

        protected override FieldToEntityPropertyMapping[] FieldMappings => new FieldToEntityPropertyMapping[]
        {
            new FieldToEntityPropertyMapping("Title", "DisplayName"),
            new FieldToEntityPropertyMapping("Dismissed"),
            new FieldToEntityPropertyMapping("Description"),
            new FieldToEntityPropertyMapping("BirthDate"),
            new FieldToEntityPropertyMapping("Department"),
            new FieldToEntityPropertyMapping("Link"),
            new FieldToEntityPropertyMapping("JobTitle"),
            new FieldToEntityPropertyMapping("Seniority")
        };
    }
}