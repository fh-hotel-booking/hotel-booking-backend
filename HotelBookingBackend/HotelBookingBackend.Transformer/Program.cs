using Avro.Generic;
using Confluent.Kafka;
using Confluent.SchemaRegistry.Serdes;
using Confluent.SchemaRegistry;
using Confluent.Kafka.SyncOverAsync;
using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.SerDes;
using Streamiz.Kafka.Net.Table;

namespace HotelBookingBackend.Transformer
{
    public class Program
    {
        private const string env_input_consumer_kafka_bootstrap = "INPUT_CONSUMER_KAFKA_BOOTSTRAP_SERVER";
        private const string env_input_consumer_topic_name = "INPUT_CONSUMER_TOPIC_NAME";
        private const string env_input_consumer_group_id = "INPUT_CONSUMER_GROUP_ID";
        private const string env_output_producer_kafka_bootstrap = "OUTPUT_PRODUCER_KAFKA_BOOTSTRAP_SERVER";
        private const string env_output_producer_topic_name = "OUTPUT_PRODUCER_TOPIC_NAME";
        private const string env_kafka_schema_server = "TRANSFORMER_KAFKA_SCHEMA_SERVER";

        public static async Task Main(string[] args)
        {
            var consumerTopicName = Environment.GetEnvironmentVariable(env_input_consumer_topic_name) ?? "booking";
            var producerTopicName = Environment.GetEnvironmentVariable(env_input_consumer_topic_name) ?? "booking-aggregate";
            var schemaServerAddress = Environment.GetEnvironmentVariable(env_kafka_schema_server);
            var running = true;

            var config = new StreamConfig() {
                ApplicationId = "bookdata-transformer",
                ClientId = "bookdata-transformer-client",
                BootstrapServers = Environment.GetEnvironmentVariable(env_input_consumer_kafka_bootstrap),
                AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest,
                SchemaRegistryUrl = Environment.GetEnvironmentVariable(env_kafka_schema_server),
            };
            var builder = new StreamBuilder();
            var bookData = builder.GlobalTable<string, BookingData, StringSerDes, JsonSerDes<BookingData>>(consumerTopicName, RocksDb.As<string, BookingData>(consumerTopicName));

            var averagePriceStream = builder.Stream<string, BookingData, StringSerDes, JsonSerDes<BookingData>>(consumerTopicName);

            /*averagePriceStream
                .GroupBy((key, val) => val.HotelName)
                .Count("hotel-count")
                .Aggregate(() => 0l, (key, val, aggregate) => { aggregate += val.Price; return aggregate; })
                ;
            /*
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable(env_input_consumer_kafka_bootstrap),
                GroupId = Environment.GetEnvironmentVariable(env_input_consumer_group_id),
            };
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable(env_output_producer_kafka_bootstrap),
            };
            var consumerTopicName = Environment.GetEnvironmentVariable(env_input_consumer_topic_name) ?? "booking";
            var producerTopicName = Environment.GetEnvironmentVariable(env_input_consumer_topic_name) ?? "booking-aggregate";
            var schemaServerAddress = Environment.GetEnvironmentVariable(env_kafka_schema_server);
            var running = true;

            using (var schemaRegistry = new CachedSchemaRegistryClient(new SchemaRegistryConfig { Url = schemaServerAddress }))
            using (var producer = new ProducerBuilder<string, BookingData>(producerConfig)
                .SetValueSerializer(new AvroSerializer<BookingData>(schemaRegistry))
                .Build())
            using (var consumer = new ConsumerBuilder<string, BookingData>(consumerConfig)
                .SetValueDeserializer(new AvroDeserializer<BookingData>(schemaRegistry).AsSyncOverAsync())
                .Build())
            {
                try
                {
                    consumer.Subscribe(consumerTopicName);
                    try
                    {
                        while (running)
                        {
                            try
                            {
                                var consumeResult = consumer.Consume();
                                consumer.Consume
                                Console.WriteLine($"Key: {consumeResult.Message.Key}\nValue: {consumeResult.Message.Value}");
                            }
                            catch (ConsumeException exc)
                            {
                                Console.WriteLine($"Consume error: {exc.Error.Reason}");
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // commit final offsets and leave the group.
                        consumer.Close();
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc);
                }
            }
            */
        }
    }
}