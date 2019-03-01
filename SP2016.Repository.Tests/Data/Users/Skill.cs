namespace SP2016.Repository.Tests.Data.Users
{
    public class Skill
    {
        public Skill()
        { }

        public Skill(string title, Seniority level)
        {
            Title = title;
            Level = level;
        }

        public string Title { get; set; }
        public Seniority Level { get; set; }
    }
}
