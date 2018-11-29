using Microsoft.VisualStudio.TestTools.UnitTesting;
using SP2016.Repository.Caml;
using System.Collections.Generic;
using System.Linq;

namespace SP2016.Repository.Tests
{
    [TestClass]
    public class FiltersTests : BaseRepoTest
    {
        [TestMethod]
        public void GetEntity_WithCustomStringCaml_ReturnsOneEntity()
        {
            Perform(web =>
            {
                try
                {
                    UsersRepository.AddRange(web, MockUsers.AllUsers);

                    var camlQuery =
                    "<Where>" +
                        "<Contains>" +
                            "<FieldRef Name=\"JobTitle\" />" +
                            "<Value Type=\"Text\">ASP</Value>" +
                        "</Contains>" +
                    "</Where>";

                    var entity = UsersRepository.GetEntity(web, camlQuery);

                    Assert.IsTrue(entity.DisplayName == "Olga M" || entity.DisplayName == "Vitaly N");
                }
                finally
                {
                    UsersRepository.DeleteAll(web);
                }
            });
        }

        [TestMethod]
        public void GetEntities_FromSpecificFolder_EqualAmount()
        {
            Perform(web => {
                try
                {
                    var folderPath = "Old men";

                    UsersRepository.Add(web, folderPath, MockUsers.User1);
                    UsersRepository.Add(web, folderPath, MockUsers.User2);

                    var oldMenEntities = UsersRepository.GetEntities(web, folderPath);

                    Assert.AreEqual(2, oldMenEntities.Length);
                }
                finally
                {
                    UsersRepository.DeleteAll(web);
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
    }
}
