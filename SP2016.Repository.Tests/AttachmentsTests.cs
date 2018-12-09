using Microsoft.VisualStudio.TestTools.UnitTesting;
using SP2016.Repository.Tests.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SP2016.Repository.Tests
{
    [TestClass]
    public class AttachmentsTests : BaseRepoTest
    {
        [TestMethod]
        public void UploadAttachment_FromLocalPath_FileWasAttached()
        {
            Perform(web =>
            {

                try
                {
                    var user = MockUsers.User1;
                    UsersRepository.Add(web, user);

                    UsersRepository.UploadAttachment(web, user, "CV", Resources.Test_document);

                    Assert.AreEqual(1, user.ListItem.Attachments.Count);
                    Assert.AreEqual("CV", user.ListItem.Attachments[0]);
                }
                finally
                {
                    UsersRepository.DeleteAll(web);
                }
            });
        }

        [TestMethod]
        public void GetAttachments_FromSpecificItemWithTwoAttachments_ReturnsRightFiles()
        {
            Perform(web =>
            {

                try
                {
                    var user = MockUsers.User2;
                    UsersRepository.Add(web, user);

                    user.ListItem.Attachments.AddNow("CV", Resources.Test_document);
                    user.ListItem.Attachments.AddNow("Timesheet", Resources.Employee_time_sheet_sample);

                    var userDocs = UsersRepository.GetAttachments(web, user);

                    Assert.AreEqual(2, userDocs.Length);

                    Assert.AreEqual("CV", userDocs[0].Name);
                    Assert.AreEqual("Timesheet", userDocs[1].Name);
                }
                finally
                {
                    UsersRepository.DeleteAll(web);
                }
            });
        }

        [TestMethod]
        public void DeleteAttachment_ByName_OnlyNeededAttachmentLeft()
        {
            Perform(web =>
            {
                try
                {
                    var user = MockUsers.User2;
                    UsersRepository.Add(web, user);

                    user.ListItem.Attachments.AddNow("CV", Resources.Test_document);
                    user.ListItem.Attachments.AddNow("Timesheet", Resources.Employee_time_sheet_sample);

                    UsersRepository.DeleteAttachment(web, user, "CV");
                    var userDocs = user.ListItem.Attachments;

                    Assert.AreEqual(1, userDocs.Count);
                    Assert.AreEqual("Timesheet", userDocs[0]);
                }
                finally
                {
                    UsersRepository.DeleteAll(web);
                }
            });
        }
    }
}
