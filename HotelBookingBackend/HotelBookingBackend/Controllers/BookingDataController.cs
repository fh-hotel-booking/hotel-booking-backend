using HotelBookingBackend.Model;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingDataController : ControllerBase
    {
 
        private readonly ILogger<BookingDataController> _logger;

        public BookingDataController(ILogger<BookingDataController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetBookingData")]
        public IEnumerable<BookingData> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new BookingData {}).ToArray();
        }
    }
}
