using HotelBookingBackend.DataAccess;
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

        [HttpGet(Name = "GetBookingData")]
        public async Task<IEnumerable<DataAccess.BookingDataDb>> Get()
        {
            return await _bookingDataService.GetAsync();
        }
    }
}
