using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace SP2016.Repository.Tests.Repository
{
    [TestClass]
    public class EntitiesAdditionTests : BaseRepoTest
    {
        [TestMethod]
        public void Add_NewEntity_ItemAddedAndFieldsAreFilled()
        {
            Perform(web => {
                try
                {
                    var user = MockUsers.User1;
                    UsersRepository.Add(web, user);

                    Assert.IsNotNull(user.ID, $"item ID is null");
                    Assert.IsNotNull(user.Created, $"item Created date is null");
                    Assert.IsNotNull(user.Author, $"item Authoris null");
                    Assert.IsTrue(user.ID != 0, $"User ID is {user.ID}");
                    Assert.IsTrue(user.ListItem.ID == user.ID);

                    var addedUser = UsersRepository.GetEntityById(web, user.ID);

                    Assert.AreEqual(user.DisplayName, addedUser.DisplayName);
                }
                finally
                {
                    UsersRepository.DeleteAll(web);
                }
            });
        }

        [TestMethod]
        public void AddBatch_CollectonOfUsers_ItemsAddedProperly()
        {
            Perform(web => {
                try
                {
                    UsersRepository.AddBatch(web, MockUsers.AllUsers);

                    var allUsers = UsersRepository.GetAllEntities(web);

                    Assert.AreEqual(allUsers.Length, MockUsers.AllUsers.Length,
                        $"There are {allUsers.Length} users while should be {MockUsers.AllUsers.Length}");

                    Assert.IsTrue(allUsers.All(u => u.ID != 0),
                        "Items IDs are not setted.");

                    Assert.IsTrue(allUsers.All(u => u.Author != null),
                        "Items Authors are not setted.");
                }
                finally
                {
                    UsersRepository.DeleteAll(web);
                }
            });
        }
    }
}
