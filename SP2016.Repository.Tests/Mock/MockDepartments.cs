namespace SP2016.Repository.Tests.Mock
{
    public class MockDepartments
    {
        public readonly DepartmentEntity Department1 = new DepartmentEntity
        {
           Title = "Software Development",
           EmployeesCount = 5
        };

        public readonly DepartmentEntity Department2 = new DepartmentEntity
        {
            Title = "QA",
            EmployeesCount = 1
        };

        public readonly DepartmentEntity[] AllDepartments;

        public MockDepartments()
        {
            AllDepartments = new DepartmentEntity[]
            {
                Department1,
                Department2
            };
        }
    }
}