using SP2016.Repository.Entities;

namespace SP2016.Repository.Tests
{
    public class DepartmentEntity : BaseEntity
    {
        public string Title { get; set; }
        public int EmployeesCount { get; set; }
    }
}