using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HotelBookingBackend.DataAccess
{
    public class BookingDataDb
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string HotelName { get; set; } = null!;
        public string RoomName { get; set; } = null!;
        public int AmountOfBeds { get; set; }
        public long Price { get; set; }
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
    }
}
