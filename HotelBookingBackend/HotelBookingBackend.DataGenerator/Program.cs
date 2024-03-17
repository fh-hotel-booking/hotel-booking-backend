using Avro.Generic;
using Bogus;
using Confluent.Kafka;
using Confluent.SchemaRegistry.Serdes;
using Confluent.SchemaRegistry;
using Confluent.Kafka.SyncOverAsync;

namespace HotelBookingBackend.DataGenerator
{
    public class Program
    {
        private const string env_producer_kafka_bootstrap = "PRODUCER_KAFKA_BOOTSTRAP_SERVER";
        private const string env_producer_wait_time_between_messages_milliseconds = "PRODUCER_WAIT_TIME_BETWEEN_MESSAGES_MILLISECONDS";
        private const string env_producer_topic_name = "PRODUCER_TOPIC_NAME";
        private const string env_producer_schema_server = "PRODUCER_KAFKA_SCHEMA_SERVER";
        private const string env_producer_schema_subject_name = "PRODUCER_KAFKA_SCHEMA_SUBJECT_NAME";

        public static async Task Main(string[] args)
        {

            var config = new ProducerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable(env_producer_kafka_bootstrap),
            };
            var topicName = Environment.GetEnvironmentVariable(env_producer_topic_name) ?? "booking";
            var waitTimeMsEnv = Environment.GetEnvironmentVariable(env_producer_wait_time_between_messages_milliseconds) ?? "100";
            var schemaServerAddress = Environment.GetEnvironmentVariable(env_producer_schema_server);
            var schemaSubjectName = Environment.GetEnvironmentVariable(env_producer_schema_subject_name);
            var waitTimeMs = int.Parse(waitTimeMsEnv);
            var running = true;

            // create fake messagges
            var fakeBookingData = new Faker<BookingData>()
                .RuleFor(m => m.HotelName, f => f.Name.FirstName())
                .RuleFor(m => m.RoomName, f => f.Name.LastName())
                .RuleFor(m => m.City, f => f.Address.City())
                .RuleFor(m => m.Country, f => f.Address.Country())
                .RuleFor(m => m.AmountOfBeds, f => f.Random.Int(1, 8))
                .RuleFor(m => m.Price, f => f.Random.Int(100, 100000));

            using (var schemaRegistry = new CachedSchemaRegistryClient(new SchemaRegistryConfig { Url = schemaServerAddress }))
            using (var producer = new ProducerBuilder<string, BookingData>(config)
                .SetValueSerializer(new AvroSerializer<BookingData>(schemaRegistry))
                .Build())
            {
                var schemaAvsc = Avro.Schema.Parse(File.ReadAllText("BookingData.avsc"));
                await schemaRegistry.RegisterSchemaAsync(schemaSubjectName, schemaAvsc.ToString());

                while (running)
                {
                    try
                    {
                        var result = await producer.ProduceAsync(
                            topicName,
                            new Message<string, BookingData>() { 
                                Key = Guid.NewGuid().ToString() , 
                                Value = fakeBookingData.Generate(),
                            }
                            );
                        Thread.Sleep(waitTimeMs);
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc);
                    }
                }
            }
        }
    }
}