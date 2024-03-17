using Avro;
using Avro.Specific;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingBackend.Transformer
{
    public class BookingData : ISpecificRecord
    {
        public string HotelName { get; set; } = "";
        public string RoomName { get; set; } = "";
        public int AmountOfBeds { get; set; }
        public long Price { get; set; }
        public string City { get; set; } = "";
        public string Country { get; set; } = "";

        public Schema Schema => Avro.Schema.Parse(File.ReadAllText("BookingData.avsc"));

        public object Get(int fieldPos)
        {
            switch (fieldPos)
            {
                case 0: return this.HotelName;
                case 1: return this.RoomName;
                case 2: return this.AmountOfBeds;
                case 3: return this.Price;
                case 4: return this.City;
                case 5: return this.Country;
                default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
            };
        }

        public void Put(int fieldPos, object fieldValue)
        {
            switch (fieldPos)
            {
                case 0: this.HotelName = (System.String)fieldValue; break;
                case 1: this.RoomName = (System.String)fieldValue; break;
                case 2: this.AmountOfBeds = (int)fieldValue; break;
                case 3: this.Price = (long)fieldValue; break;
                case 4: this.City = (System.String)fieldValue; break;
                case 5: this.Country = (System.String)fieldValue; break;
                default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
            };
        }

        public override string? ToString()
        {
            return $"{HotelName} {Price}";
        }
    }
}
