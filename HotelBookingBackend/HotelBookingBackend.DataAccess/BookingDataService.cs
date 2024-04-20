using MongoDB.Driver;

namespace HotelBookingBackend.DataAccess
{
    public class BookingDataService
    {
        private const string env_mongoDb_connectionString = "MONGO_DB_CONNECTION_STRING";

        private readonly string _connectionString;
        private readonly string _dataBaseName = "HotelBooking";
        private readonly string _hotelCollectionName = "Hotels";
        private readonly IMongoCollection<BookingDataDb> _bookingDataCollection;

        public BookingDataService()
        {
            _connectionString = Environment.GetEnvironmentVariable(env_mongoDb_connectionString) ?? "mongodb://user:pass@localhost:27017/";
            MongoClient dbClient = new(_connectionString);
            IMongoDatabase mongoDatabase = dbClient.GetDatabase(_dataBaseName);

            _bookingDataCollection = mongoDatabase.GetCollection<BookingDataDb>(_hotelCollectionName
          );
        }
        public async Task<List<BookingDataDb>> GetAsync()
        {
            return await _bookingDataCollection.Find(_ => true).ToListAsync();
        }

        public async Task CreateAsync(BookingDataDb newBook)
        {
            await _bookingDataCollection.InsertOneAsync(newBook);
        }

        public async Task<BookingDataDb?> GetAsync(string id)
        {
            return await _bookingDataCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(string id, BookingDataDb updatedBook)
        {
            await _bookingDataCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);
        }

        public async Task RemoveAsync(string id)
        {
            await _bookingDataCollection.DeleteOneAsync(x => x.Id == id);
        }
    }
}
