namespace SP2016.Repository.Tests
{
    public class DepartmentsRepository : SharePointRepository<DepartmentEntity>
    {
        public override string ListName => "Departments";
    }
}