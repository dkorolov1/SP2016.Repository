using SP2016.Repository.Attributes;
using SP2016.Repository.Entities;
using SP2016.Repository.Tests.CustomConverters;

namespace SP2016.Repository.Tests.Data.Rooms
{
    public class RoomEntity : BaseSPEntity
    {
        [FieldMapping("Title", typeof(InvalidConverter))]
        public string Title { get; set; }
    }
}
