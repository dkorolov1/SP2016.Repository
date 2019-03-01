using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace SP2016.Repository.Tests.Repository
{
    [TestClass]
    public class FiltersTests : BaseRepoTest
    {
        [TestMethod]
        public void GetEntities_FromSpecificFolder_EqualAmount()
        {
            var folderPath = "Old men";

            Perform(web => {
                try
                {
                    UsersRepository.Add(web, folderPath, MockUsers.User1);
                    UsersRepository.Add(web, folderPath, MockUsers.User2);

                    var oldMenEntities = UsersRepository.GetEntities(web, folderPath);

                    Assert.AreEqual(2, oldMenEntities.Length);
                }
                finally
                {
                    UsersRepository.DeleteAll(web);
                    UsersRepository.DeleteFolder(web, folderPath);
                }
            });
        }

        [TestMethod]
        public void GetEntities_QueryByJobTitle_EqualAmountAndJobTitles()
        {
            Perform(web => {
                try
                {
                    UsersRepository.AddRange(web, MockUsers.AllUsers);

                    var aspDevelopers = UsersRepository.GetByJobTitle(web, "ASP");


                    Assert.AreEqual(
                        MockUsers.AllUsers.Count(d => d.JobTitle.Contains("ASP")),
                        aspDevelopers.Length);

                    Assert.IsTrue(aspDevelopers.ToList().All(d => d.JobTitle.Contains("ASP")));
                }
                finally
                {
                    UsersRepository.DeleteAll(web);
                }
            });
        }

        [TestMethod]
        public void GetEntities_QueryWithRowLimit_ReturnsCorrectItemsAmount()
        {
            Perform(web =>
            {
                try
                {
                    UsersRepository.AddRange(web, MockUsers.AllUsers);

                    uint rowLimit = 1;
                    var aspDevelopersWithLimit = UsersRepository.GetByJobTitle(web, "ASP", rowLimit);

                    Assert.IsTrue(aspDevelopersWithLimit.Length <= rowLimit, $"Row limit is {rowLimit}.");
                }
                finally
                {
                    UsersRepository.DeleteAll(web);
                }
            });
        }

        [TestMethod]
        public void GetEntities_GetByNameUsingNativeSPCaml_OnePerson()
        {
            Perform(web =>
            {
                try
                {
                    UsersRepository.AddRange(web, MockUsers.AllUsers);

                    uint rowLimit = 1;
                    var aspDevelopersWithLimit = UsersRepository.GetByJobTitle(web, "ASP", rowLimit);

                    Assert.IsTrue(aspDevelopersWithLimit.Length <= rowLimit, $"Row limit is {rowLimit}.");
                }
                finally
                {
                    UsersRepository.DeleteAll(web);
                }
            });
        }

        [TestMethod]
        public void GetEntitiesByTitle_WithSpecificTitle_RightItem()
        {
            Perform(web =>
            {
                try
                {
                    UsersRepository.AddRange(web, MockUsers.AllUsers);

                    var developer = UsersRepository.GetEntitiesByTitle(web, MockUsers.User1.DisplayName);

                    Assert.IsTrue(developer.All(d => d.DisplayName == MockUsers.User1.DisplayName));

                }
                finally
                {
                    UsersRepository.DeleteAll(web);
                }
            });
        }

        [TestMethod]
        public void GetEntitiesByTitle_WithSpecificTitleAndRowLimit_RightItem()
        {
            Perform(web =>
            {
                var folderName = "Fresh people";

                try
                {
                    UsersRepository.Add(web, folderName, MockUsers.User2);
                    UsersRepository.Add(web, folderName, MockUsers.User3);

                    uint rowLimit = 1;
                    var developers = UsersRepository.GetEntitiesByTitle(web, MockUsers.User2.DisplayName, true, rowLimit);

                    Assert.IsTrue(rowLimit == developers.Length,
                        $"Recived {developers.Length} developers while row limit is {rowLimit}");

                    Assert.IsTrue(
                        developers[0].DisplayName == MockUsers.User2.DisplayName ||
                        developers[0].DisplayName == MockUsers.User2.DisplayName);

                }
                finally
                {
                    UsersRepository.DeleteAll(web);
                    UsersRepository.DeleteFolder(web, folderName);
                }
            });
        }

        [TestMethod]
        public void GetBySeniorityAndJobTitle_WithSpecificSeniorityAndJobKeyWord_ReturnsOneEmployee()
        {
            Perform(web =>
            {
                try
                {
                    UsersRepository.AddRange(web, MockUsers.AllUsers);

                    var aspMiddleDevelopers = UsersRepository.GetBySeniorityAndJobTitle(web, Data.Users.Seniority.Middle, "ASP");

                    var olga = aspMiddleDevelopers[0];

                    Assert.AreEqual(1, aspMiddleDevelopers.Length);
                    Assert.AreEqual("Olga M", olga.DisplayName);

                }
                finally
                {
                    UsersRepository.DeleteAll(web);
                }
            });
        }

        [TestMethod]
        public void GetByAgeWithDisplayName_WithSpecificAge_ReturnsItemsWithDisplayNameOnly()
        {
            Perform(web =>
            {
                try
                {
                    UsersRepository.AddRange(web, MockUsers.AllUsers);

                    var adultDevelopers = UsersRepository.GetByAgeWithDisplayName(web, 23);

                    Assert.AreEqual(2, adultDevelopers.Length);

                    Assert.IsNotNull(adultDevelopers[0].DisplayName);

                }
                finally
                {
                    UsersRepository.DeleteAll(web);
                }
            });
        }
    }
}
