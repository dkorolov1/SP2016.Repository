using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SP2016.Repository.Tests.Repository
{
    [TestClass]
    public class GettingLinksToTheFormsTests : BaseRepoTest
    {
        [TestMethod]
        public void GetDefaultViewUrl_CompareToStaticUrl_UrlsAreEqual()
        {
            Perform(web =>
            {
                const string expectedUrl = "/training/Lists/Users/AllItems.aspx";
                var actualUrl = UsersRepository.GetDefaultViewUrl(web);

                Assert.AreEqual(expectedUrl, actualUrl);
            });
        }

        [TestMethod]
        public void GetFullDisplayFormUrl_OfSpecificItem_ProperUrl()
        {
            Perform(web =>
            {
                try
                {
                    var user = MockUsers.User1;
                    UsersRepository.Add(web, user);

                    var expectedUrl = $"/training/Lists/Users/DispForm.aspx?ID={user.ID}";

                    var actualUrl = UsersRepository.GetDisplayFormUrl(web, user.ID);

                    Assert.AreEqual(expectedUrl, actualUrl);
                }
                finally
                {
                    UsersRepository.DeleteAll(web);
                }
            });
        }

        // TODO: finish URLs methods tests.
    }
}
