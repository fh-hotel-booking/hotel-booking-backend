using Avro.Generic;
using Confluent.Kafka;
using Confluent.SchemaRegistry.Serdes;
using Confluent.SchemaRegistry;
using Confluent.Kafka.SyncOverAsync;

namespace HotelBookingBackend.Transformer
{
    public class Program
    {
        private const string env_input_consumer_kafka_bootstrap = "INPUT_CONSUMER_KAFKA_BOOTSTRAP_SERVER";
        private const string env_input_consumer_topic_name = "INPUT_CONSUMER_TOPIC_NAME";
        private const string env_output_producer_kafka_bootstrap = "OUTPUT_PRODUCER_KAFKA_BOOTSTRAP_SERVER";
        private const string env_output_producer_topic_name = "OUTPUT_PRODUCER_TOPIC_NAME";
        private const string env_kafka_schema_server = "TRANSFORMER_KAFKA_SCHEMA_SERVER";

        public static async Task Main(string[] args)
        {

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable(env_input_consumer_kafka_bootstrap),
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
            using (var producer = new ProducerBuilder<Null, BookingData>(producerConfig)
                .Build())
            using (var consumer = new ConsumerBuilder<Null, BookingData>(consumerConfig)
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
        }
    }
}