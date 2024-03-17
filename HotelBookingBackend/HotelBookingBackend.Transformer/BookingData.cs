using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingBackend.Transformer
{
    public class BookingData
    {
        public string Name { get; set; } = "";
        public long AmountOfBeds { get; set; }
        public long Price { get; set; }
    }
}
