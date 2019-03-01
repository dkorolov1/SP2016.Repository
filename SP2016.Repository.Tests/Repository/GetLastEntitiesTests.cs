using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SP2016.Repository.Tests.Repository
{
    [TestClass]
    public class GetLastEntitiesTests : BaseRepoTest
    {
        [TestMethod]
        public void GetLastEntities_LastTwo_ReturnsTwoInProperOrder()
        {
            Perform(web => {
                try
                {
                    UsersRepository.Add(web, MockUsers.User3);
                    UsersRepository.Add(web, MockUsers.User4);
                    UsersRepository.Add(web, MockUsers.User1);

                    uint numberOfNeededUsers = 2;
                    var lastCreatedUsers = UsersRepository.GetLastEntities(web, numberOfNeededUsers);

                    Assert.AreEqual((int)numberOfNeededUsers, lastCreatedUsers.Length,
                        $"Expected {numberOfNeededUsers} users to receive, but {lastCreatedUsers.Length} received");

                    Assert.AreEqual(MockUsers.User1.DisplayName, lastCreatedUsers[0].DisplayName);
                }
                finally
                {
                    UsersRepository.DeleteAll(web);
                }
            });
        }
    }
}
