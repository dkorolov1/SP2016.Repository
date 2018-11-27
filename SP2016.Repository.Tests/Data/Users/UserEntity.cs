using Microsoft.SharePoint;
using SP2016.Repository.Entities;
using SP2016.Repository.Tests.Data.Users;
using System;

namespace SP2016.Repository.Tests
{
    public class UserEntity : BaseEntity
    {
        public string DisplayName { get; set; }

        public bool Dismissed { get; set; }

        public string Description { get; set; }

        public DateTime? BirthDate { get; set; }

        public SPFieldLookupValue Department { get; set; }

        public SPFieldUrlValue Link { get; set; }

        public string JobTitle { get; set; }

        public Seniority Seniority { get; set; }
    }
}