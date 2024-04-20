using Avro;
using Avro.Specific;

namespace HotelBookingBackend.Consumer
{
    public class BookingData : ISpecificRecord
    {
        public string HotelName { get; set; } = "";
        public string RoomName { get; set; } = "";
        public int AmountOfBeds { get; set; }
        public long Price { get; set; }
        public string City { get; set; } = "";
        public string Country { get; set; } = "";

        public Schema Schema => Schema.Parse(File.ReadAllText("BookingData.avsc"));

        public object Get(int fieldPos)
        {
            switch (fieldPos)
            {
                case 0: return HotelName;
                case 1: return RoomName;
                case 2: return AmountOfBeds;
                case 3: return Price;
                case 4: return City;
                case 5: return Country;
                default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
            };
        }

        public void Put(int fieldPos, object fieldValue)
        {
            switch (fieldPos)
            {
                case 0: HotelName = (string)fieldValue; break;
                case 1: RoomName = (string)fieldValue; break;
                case 2: AmountOfBeds = (int)fieldValue; break;
                case 3: Price = (long)fieldValue; break;
                case 4: City = (string)fieldValue; break;
                case 5: Country = (string)fieldValue; break;
                default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
            };
        }

        public override string? ToString()
        {
            return $"{HotelName} {Price}";
        }
    }
}