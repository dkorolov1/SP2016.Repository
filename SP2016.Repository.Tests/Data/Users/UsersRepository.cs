using Microsoft.SharePoint;
using SP2016.Repository.Caml;
using SP2016.Repository.Mapping;
using SP2016.Repository.Enums;
using SP2016.Repository.Tests.Data.Users;
using System;

namespace SP2016.Repository.Tests
{
    public class UsersRepository : SharePointRepository<UserEntity>
    {
        public override string ListName => "Users";

        public UserEntity[] GetByJobTitle(SPWeb web, string jobTitle, uint rowLimit = 0)
        {
            var jobTitleFilter = new Filter(FilterType.Contains, "JobTitle", jobTitle, FilterValueType.Text);

            return GetEntities(web, jobTitleFilter, true, rowLimit);
        }

        public UserEntity[] GetBySeniorityAndJobTitle(SPWeb web, Seniority seniority, string jobTitle)
        {
            var jobTitleFilter = new Filter(FilterType.Contains, "JobTitle", jobTitle, FilterValueType.Text);
            var seniorityFilter = new Filter(FilterType.Contains, "Seniority", seniority.ToString(), FilterValueType.Text);

            return GetEntities(web, jobTitleFilter * seniorityFilter, false);
        }

        public UserEntity[] GetByAgeWithDisplayName(SPWeb web, int age)
        {
            var birthDateYear = DateTime.Now.AddYears(-1 * age);

            var ageFilter = new Filter(FilterType.GreaterThan, "BirthDate", birthDateYear, FilterValueType.DateTime);

            return GetEntities(web, ageFilter, false);
        }
    }
}