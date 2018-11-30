using SP2016.Repository.Mapping;

namespace SP2016.Repository.Tests
{
    public class DepartmentsRepository : BaseEntityRepository<UserEntity>
    {
        public override string ListName => "Departments";

        protected override FieldToPropertyMapping[] FieldMappings => new FieldToPropertyMapping[]
        {
            new FieldToPropertyMapping("Title"),
            new FieldToPropertyMapping("EmployeesCount")
        };
    }
}