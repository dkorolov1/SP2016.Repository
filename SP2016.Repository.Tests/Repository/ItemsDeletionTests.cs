using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace SP2016.Repository.Tests.Repository
{
    [TestClass]
    public class ItemsDeletionTests : BaseRepoTest
    {
        [TestMethod]
        public void DeleteBatch_DeleteSpecificItems_OtherItemsStillExist()
        {
            Perform(web => {
                try
                {
                    UsersRepository.AddBatch(web, MockUsers.AllUsers);
                    var usersToBeDeleted = new UserEntity[] { MockUsers.User2, MockUsers.User4 };

                    UsersRepository.DeleteBatch(usersToBeDeleted, web);

                    var remainingEntities = UsersRepository.GetAllEntities(web);

                    Assert.AreEqual(2, remainingEntities.Length);

                    Assert.IsTrue(remainingEntities.Any(u => u.DisplayName == MockUsers.User1.DisplayName));
                    Assert.IsTrue(remainingEntities.Any(u => u.DisplayName == MockUsers.User3.DisplayName));
                }
                finally
                {
                    UsersRepository.DeleteAll(web);
                }
            });
        }
    }
}
