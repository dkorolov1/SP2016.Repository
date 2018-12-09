using Microsoft.SharePoint.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SP2016.Repository.Tests
{
    [TestClass]
    public class GettingLinksToTheFormsTests : BaseRepoTest
    {
        [TestMethod]
        public void GetDefaultViewUrl_CompareToStaticUrl_UrlsAreEqual()
        {
            Perform(web =>
            {
                string webServerRelativeUrl = web.ServerRelativeUrl;

                string viewWebRelativeUrl_expected = "Lists/Users/AllItems.aspx";
                string viewServerRelativeUrl_expected = 
                    SPUtility.ConcatUrls(webServerRelativeUrl, viewWebRelativeUrl_expected);

                var viewServerRelativeUrl_actual = UsersRepository.GetDefaultViewUrl(web);

                Assert.AreEqual(viewServerRelativeUrl_expected, viewServerRelativeUrl_actual);
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

                    string webServerRelativeUrl = web.ServerRelativeUrl;
                    string dispFormWebRelativeUrl_expected = $"Lists/Users/DispForm.aspx?ID={user.ID}";

                    var dispFormServerRelativeUrl_expected = 
                        SPUtility.ConcatUrls(webServerRelativeUrl, dispFormWebRelativeUrl_expected);

                    var dispFormServerRelativeUrl_actual = UsersRepository.GetDisplayFormUrl(web, user.ID);

                    Assert.AreEqual(dispFormServerRelativeUrl_expected, dispFormServerRelativeUrl_actual);
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
