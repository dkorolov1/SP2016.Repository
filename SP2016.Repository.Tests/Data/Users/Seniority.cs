using System.ComponentModel;

namespace SP2016.Repository.Tests.Data.Users
{
    public enum Seniority
    {
        [Description("Junior Developer")]
        Junior = 1,
        [Description("Middle Developer")]
        Middle = 2,
        [Description("Senior Developer")]
        Senior = 3
    }
}