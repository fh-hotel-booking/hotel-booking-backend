using HotelBookingBackend.DataAccess;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingDataController : ControllerBase
    {

        private readonly ILogger<BookingDataController> _logger;

        private readonly BookingDataService _bookingDataService;

        public BookingDataController(ILogger<BookingDataController> logger, BookingDataService bookingDataService)
        {
            _logger = logger;
            _bookingDataService = bookingDataService;
        }
        
        [EnableCors]
        [HttpGet(Name = "GetBookingData")]
        public async Task<IEnumerable<DataAccess.BookingDataDb>> Get([FromQuery(Name = "hotelName")] string? hotelName)
        {
            if (hotelName == null)
            {
                return await _bookingDataService.GetAsync();
            }

            return await _bookingDataService.GetAsyncFilter(hotelName);
        }
    }
}
