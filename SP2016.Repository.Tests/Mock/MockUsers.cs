using Microsoft.SharePoint;
using SP2016.Repository.Tests.Data.Users;
using System;
using System.Collections.Generic;

namespace SP2016.Repository.Tests.Mock
{
    public class MockUsers
    {
        public readonly UserEntity User1 = new UserEntity
        {
            DisplayName = "Dmitry K",
            Dismissed = false,
            Description = "Works with SharePoint",
            BirthDate = new DateTime(1994, 11, 25),
            Link = null,
            JobTitle = "Software Engineer (SharePoint)",
            Seniority = Seniority.Junior,
            Skills = new Skill[]
            {
                new Skill("PowerApps", Seniority.Senior),
                new Skill("SharePoint Online", Seniority.Middle)
            },
            EducationLevel = "Higher education"
        };

        public readonly UserEntity User2 = new UserEntity
        {
            DisplayName = "Vlad L",
            Dismissed = false,
            Description = "Works with React.Js",
            BirthDate = new DateTime(1998, 9, 20),
            Link = new SPFieldUrlValue()
            {
                Description = "Profile in Linkedin",
                Url = "https://www.linkedin.com/in/"
            },
            JobTitle = "Software Engineer (React.Js)",
            Seniority = Seniority.Senior,
            Skills = new Skill[]
            {
                new Skill("ASP.NET", Seniority.Junior),
                new Skill("React.js", Seniority.Junior)
            }
        };

        public readonly UserEntity User3 = new UserEntity
        {
            DisplayName = "Olga M",
            Dismissed = false,
            Description = "Works with ASP.NET MVC",
            BirthDate = new DateTime(1997, 3, 18),
            Link = new SPFieldUrlValue()
            {
                Description = "Profile in FB",
                Url = "https://www.facebook.com/"
            },
            JobTitle = "Software Engineer (ASP.NET MVC)",
            Seniority = Seniority.Middle,
            Skills = new Skill[]
            {
                new Skill("ASP.NET", Seniority.Senior),
                new Skill("PowerBI", Seniority.Middle)
            }
        };

        public readonly UserEntity User4 = new UserEntity
        {
            DisplayName = "Vitaly N",
            Dismissed = false,
            Description = "Works with ASP.NET Core",
            BirthDate = new DateTime(1992, 8, 12),
            Link = new SPFieldUrlValue()
            {
                Description = "Profile in Skype",
                Url = "https://www.skype.com/"
            },
            JobTitle = "Software Engineer (ASP.NET Core)",
            Seniority = Seniority.Junior,
            Skills = new Skill[]
            {
                new Skill("SharePoint Server", Seniority.Middle),
                new Skill("React.js", Seniority.Junior)
            }
        };

        public readonly UserEntity[] AllUsers;

        public MockUsers()
        {
            AllUsers = new UserEntity[]
            {
                User1, User2,
                User3, User4
            };
        }
    }
}