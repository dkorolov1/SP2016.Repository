using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP2016.Repository.Tests
{
    [TestClass]
    public class UpdatingItemsTests : BaseRepoTest
    {
        [TestMethod]
        public void Update_SingleEntity_NewInfoWasRecorded()
        {
            Perform(web =>
            {
                try
                {
                    var user = MockUsers.User2;
                    UsersRepository.Add(web, user);

                    var birthdate = new DateTime(1998, 9, 20);
                    user.BirthDate = birthdate;
                    UsersRepository.Update(web, user);

                    var updatedUser = UsersRepository.GetEntityById(web, user.ID);

                    Assert.AreEqual(birthdate, updatedUser.BirthDate,
                        $"User's birth date is {updatedUser.BirthDate.ToString()} while should be {birthdate.ToString()}");
                }
                finally
                {
                    UsersRepository.DeleteAll(web);
                }
            });
        }

        [TestMethod]
        public void UpdateBatch_ItemsSet_ItemsUpdatedProperly()
        {
            Perform(web =>
            {
                try
                {
                    UsersRepository.AddBatch(web, MockUsers.AllUsers);
                    var usersToBeUpdated = new UserEntity[] { MockUsers.User2, MockUsers.User4 };

                    usersToBeUpdated[0].Dismissed = true;
                    usersToBeUpdated[1].Dismissed = true;
                    UsersRepository.UpdateBatch(usersToBeUpdated, web);

                    var allEntities = UsersRepository.GetAllEntities(web);

                    Assert.AreEqual(MockUsers.AllUsers.Length, allEntities.Length);
                    Assert.AreEqual(2, allEntities.Count(u => u.Dismissed == true));
                }
                finally
                {
                    UsersRepository.DeleteAll(web);
                }
            });
        }
    }
}
