using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace HotelBookingBackend.Consumer
{
    public class ConsumerService : BackgroundService
    {
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private readonly ConsumerConfig _consumerConfig;
        private readonly string _topic;
        private readonly double _maxNumAttempts;
        private readonly double _retryIntervalInSec;
        private readonly string _schemaServerAddress;

        private const string env_producer_kafka_bootstrap = "PRODUCER_KAFKA_BOOTSTRAP_SERVER";
        private const string env_producer_wait_time_between_messages_milliseconds = "PRODUCER_WAIT_TIME_BETWEEN_MESSAGES_MILLISECONDS";
        private const string env_producer_topic_name = "PRODUCER_TOPIC_NAME";
        private const string env_producer_schema_server = "PRODUCER_KAFKA_SCHEMA_SERVER";
        private const string env_producer_schema_subject_name = "PRODUCER_KAFKA_SCHEMA_SUBJECT_NAME";

        public ConsumerService(IConfiguration config, ILogger logger)
        {
            _config = config;
            _logger = logger;
            _consumerConfig = new ConsumerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable(env_producer_kafka_bootstrap),
                //GroupId = _config.GetValue<string>("Kafka:GroupId"),
                //EnableAutoCommit = _config.GetValue<bool>("Kafka:Consumer:EnableAutoCommit"),
                //AutoOffsetReset = (AutoOffsetReset)_config.GetValue<int>("Kafka:Consumer:AutoOffsetReset")
                EnableAutoCommit = false
            };
            _topic = Environment.GetEnvironmentVariable(env_producer_topic_name) ?? "booking";
            _maxNumAttempts = 5;
            _retryIntervalInSec = 60;
            _schemaServerAddress = Environment.GetEnvironmentVariable(env_producer_schema_server);
        }



        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("!!! CONSUMER STARTED !!!\n");

            // Starting a new Task here because Consume() method is synchronous
            Task task = Task.Run(() => ProcessQueue(stoppingToken), stoppingToken);

            return task;
        }

        private async void ProcessQueue(CancellationToken stoppingToken)
        {



            using CachedSchemaRegistryClient schemaRegistry = new(new SchemaRegistryConfig { Url = _schemaServerAddress });
            using IConsumer<string, BookingData> consumer = new ConsumerBuilder<string, BookingData>(_consumerConfig)
            .SetValueDeserializer(new AvroDeserializer<BookingData>(schemaRegistry).AsSyncOverAsync())
            .Build();
            consumer.Subscribe(_topic);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        ConsumeResult<string, BookingData> consumeResult = consumer.Consume(stoppingToken);

                        // Don't want to block consume loop, so starting new Task for each message  

                        int currentNumAttempts = 0;
                        bool committed = false;


                        while (currentNumAttempts < _maxNumAttempts)
                        {
                            currentNumAttempts++;

                            // SendDataAsync is a method that sends http request to some end-points 
                            //@TODO: Impl database saving
                            var response = await Helper.SendDataAsync(consumeResult.Value, _config, _logger);

                            if (response != null && response.Code >= 0)
                            {
                                try
                                {
                                    consumer.Commit(consumeResult);
                                    committed = true;

                                    break;
                                }
                                catch (KafkaException ex)
                                {
                                    // log
                                }
                            }
                            else
                            {
                                // log
                            }

                            if (currentNumAttempts < _maxNumAttempts)
                            {
                                // Delay between tries
                                //@TODO: Impl wait time
                            }
                        }

                        if (!committed)
                        {
                            try
                            {
                                consumer.Commit(consumeResult);
                            }
                            catch (KafkaException ex)
                            {
                                // log
                            }
                        }

                    }
                    catch (ConsumeException ex)
                    {
                        // log
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                // log
                consumer.Close();
            }



        }
    }
}
