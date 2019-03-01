using SP2016.Repository.Tests.Data.Rooms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SP2016.Repository.Tests.Repository
{
    [TestClass]
    public class MappingTests : BaseRepoTest
    {
        [TestMethod]
        public void CustomPropertyConverter_WithInvalidConverter_ThrownArgumentException()
        {
            Perform((web) =>
            {
                try
                {
                    var mockRoom = new RoomEntity
                    {
                        Title = "1201"
                    };


                    Assert.ThrowsException<ArgumentException>(() =>
                    {
                        RoomsRepository.Add(web, mockRoom);
                    });
                }
                finally
                {
                    RoomsRepository.DeleteAll(web);
                }
            });
        }
    }
}
