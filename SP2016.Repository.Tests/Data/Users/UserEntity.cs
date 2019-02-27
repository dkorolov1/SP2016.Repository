using Microsoft.SharePoint;
using SP2016.Repository.Attributes;
using SP2016.Repository.Converters.Common;
using SP2016.Repository.Entities;
using SP2016.Repository.Tests.Data.Users;
using System;
using System.Collections.Generic;

namespace SP2016.Repository.Tests
{
    public class UserEntity : BaseSPEntity
    {
        [FieldMapping("Title")]
        public string DisplayName { get; set; }

        [FieldMapping("Dismissed")]
        public bool Dismissed { get; set; }

        [FieldMapping("Description")]
        public string Description { get; set; }

        [FieldMapping("BirthDate")]
        public DateTime? BirthDate { get; set; }

        [FieldMapping("Department")]
        public SPFieldLookupValue Department { get; set; }

        [FieldMapping("Link")]
        public SPFieldUrlValue Link { get; set; }

        [FieldMapping("JobTitle")]
        public string JobTitle { get; set; }

        [FieldMapping("Seniority")]
        public Seniority Seniority { get; set; }

        [FieldMapping("Skills", typeof(JsonConverter<Dictionary<string, Seniority>>))]
        public Dictionary<string, Seniority> Skills { get; set; }
    }
}