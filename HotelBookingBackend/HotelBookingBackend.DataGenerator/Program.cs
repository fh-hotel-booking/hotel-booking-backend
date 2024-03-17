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

        public static async Task Main(string[] args)
        {

            var config = new ProducerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable(env_producer_kafka_bootstrap),
            };
            var topicName = Environment.GetEnvironmentVariable(env_producer_topic_name) ?? "booking";
            var waitTimeMsEnv = Environment.GetEnvironmentVariable(env_producer_wait_time_between_messages_milliseconds) ?? "100";
            var schemaServerAddress = Environment.GetEnvironmentVariable(env_producer_schema_server);
            var waitTimeMs = int.Parse(waitTimeMsEnv);
            var running = true;

            // create fake messagges
            var fakeBookingData = new Faker<BookingData>()
                .RuleFor(m => m.Name, f => f.Name.FullName())
                .RuleFor(m => m.AmountOfBeds, f => f.Random.Int(1, 8))
                .RuleFor(m => m.Price, f => f.Random.Int(100, 100000));

            using (var schemaRegistry = new CachedSchemaRegistryClient(new SchemaRegistryConfig { Url = schemaServerAddress }))
            using (var producer = new ProducerBuilder<Null, BookingData>(config)
                .SetValueSerializer(new AvroSerializer<BookingData>(schemaRegistry))
                .Build())
            {
                while (running)
                {
                    try
                    {
                        var result = await producer.ProduceAsync(
                            topicName,
                            new Message<Null, BookingData>() { Value = fakeBookingData.Generate() }
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