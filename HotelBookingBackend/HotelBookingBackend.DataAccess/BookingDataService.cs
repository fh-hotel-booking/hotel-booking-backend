using MongoDB.Driver;

namespace HotelBookingBackend.DataAccess
{
    public class BookingDataService
    {
        private readonly string _connectionString = "mongodb://user:pass@localhost:27017/";
        private readonly string _dataBaseName = "HotelBooking";
        private readonly string _hotelCollectionName = "Hotels";
        private readonly IMongoCollection<BookingData> _bookingDataCollection;

        public BookingDataService()
        {
            MongoClient dbClient = new(_connectionString);
            IMongoDatabase mongoDatabase = dbClient.GetDatabase(_dataBaseName);

            _bookingDataCollection = mongoDatabase.GetCollection<BookingData>(_hotelCollectionName
          );
        }
        public async Task<List<BookingData>> GetAsync()
        {
            return await _bookingDataCollection.Find(_ => true).ToListAsync();
        }

        public async Task CreateAsync(BookingData newBook)
        {
            await _bookingDataCollection.InsertOneAsync(newBook);
        }

        public async Task<BookingData?> GetAsync(string id)
        {
            return await _bookingDataCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(string id, BookingData updatedBook)
        {
            await _bookingDataCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);
        }

        public async Task RemoveAsync(string id)
        {
            await _bookingDataCollection.DeleteOneAsync(x => x.Id == id);
        }
    }
}
