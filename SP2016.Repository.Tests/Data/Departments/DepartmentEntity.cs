using SP2016.Repository.Attributes;
using SP2016.Repository.Entities;

namespace SP2016.Repository.Tests
{
    public class DepartmentEntity : BaseSPEntity
    {
        [FieldMapping("Title")]
        public string Title { get; set; }

        [FieldMapping("EmployeesCount")]
        public int EmployeesCount { get; set; }
    }
}