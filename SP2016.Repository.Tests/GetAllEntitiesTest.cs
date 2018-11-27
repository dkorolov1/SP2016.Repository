using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

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
        public void GetAllEntitiesRecursiveTest()
        {
            Perform(web => {
                string folderName = "New Employees";

                UsersRepository.AddRange(web, MockUsers.AllUsers);
                UsersRepository.AddListItemToFolder(web, folderName, MockUsers.User1);
                var items = UsersRepository.GetAllEntities(web, true);

                Debug.Assert(items.Length == MockUsers.AllUsers.Length + 1);

                UsersRepository.DeleteAll(web);
                UsersRepository.DeleteFolder(web, folderName);
            });
        }

        /// <summary>
        /// Check GetAllEntities(SPWeb web)
        /// </summary>
        [TestMethod]
        public void GetAllEntitiesTest()
        {
            Perform(web => {
                UsersRepository.AddRange(web, MockUsers.AllUsers);
                var items = UsersRepository.GetAllEntities(web);

                Debug.Assert(items.Length == MockUsers.AllUsers.Length);

                UsersRepository.DeleteAll(web);
            });
        }
    }
}
