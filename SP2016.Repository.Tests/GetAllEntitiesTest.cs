using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SP2016.Repository.Tests
{
    /// <summary>
    /// Tests for methods:
    /// GetAllEntities(SPWeb web)
    /// GetAllEntities(SPWeb web, bool recursive = false)
    /// </summary>
    [TestClass]
    public class GetAllEntitiesTests : BaseRepoTest
    {
        /// <summary>
        /// Check for GetAllEntities(SPWeb web, bool recursive = false)
        /// </summary>
        [TestMethod]
        public void GetAllEntities_Recursive_SameItemsAmount()
        {
            Perform(web => {
                string folderName = "New Employees";

                UsersRepository.AddRange(web, MockUsers.AllUsers);
                UsersRepository.Add(web, folderName, MockUsers.User1);
                var items = UsersRepository.GetAllEntities(web);

                Assert.AreEqual(items.Length, MockUsers.AllUsers.Length + 1);

                UsersRepository.DeleteAll(web);
                UsersRepository.DeleteFolder(web, folderName);
            });
        }

        /// <summary>
        /// Check GetAllEntities(SPWeb web)
        /// </summary>
        [TestMethod]
        public void GetAllEntities_NotRecursive_SameItemsAmount()
        {
            Perform(web => {
                UsersRepository.AddRange(web, MockUsers.AllUsers);
                var items = UsersRepository.GetAllEntities(web, false);

                Assert.AreEqual(items.Length, MockUsers.AllUsers.Length);

                UsersRepository.DeleteAll(web);
            });
        }
    }
}
