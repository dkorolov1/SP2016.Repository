namespace SP2016.Repository.Tests.Data.Rooms
{
    public class RoomsRepository : SharePointRepository<RoomEntity>
    {
        public override string ListName => "Rooms";
    }
}
