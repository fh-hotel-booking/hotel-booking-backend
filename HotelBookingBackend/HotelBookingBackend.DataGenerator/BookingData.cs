using Avro;
using Avro.Specific;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingBackend.DataGenerator
{
    public class BookingData : ISpecificRecord
    {
        public string Name { get; set; } = "";
        public long AmountOfBeds { get; set; }
        public long Price { get; set; }

        public Schema Schema => throw new NotImplementedException();

        public object Get(int fieldPos)
        {
            throw new NotImplementedException();
        }

        public void Put(int fieldPos, object fieldValue)
        {
            throw new NotImplementedException();
        }
    }
}
