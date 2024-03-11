using Bogus;
using Confluent.Kafka;

public class Program
{
    private const string env_producer_kafka_bootstrap = "PRODUCER_KAFKA_BOOTSTRAP_SERVER";
    private const string env_producer_wait_time_between_messages_milliseconds = "PRODUCER_WAIT_TIME_BETWEEN_MESSAGES_MILLISECONDS";
    private const string env_producer_topic_name = "PRODUCER_TOPIC_NAME";

    public static async Task Main(string[] args)
    {

        var config = new ProducerConfig
        {
            BootstrapServers = Environment.GetEnvironmentVariable(env_producer_kafka_bootstrap),
        };
        var topicName = Environment.GetEnvironmentVariable(env_producer_topic_name) ?? "booking";
        var waitTimeMsEnv = Environment.GetEnvironmentVariable(env_producer_wait_time_between_messages_milliseconds) ?? "100";
        var waitTimeMs = int.Parse(waitTimeMsEnv);
        var running = true;

        // create fake messagges
        var fakeMessage = new Faker<Message<Null, string>>()
            .RuleFor(m => m.Value, f => f.Name.FullName());

        using (var producer = new ProducerBuilder<Null, string>(config).Build())
        {
            while (running) {
                try
                {
                    var result = await producer.ProduceAsync(topicName, fakeMessage.Generate());
                    Thread.Sleep(waitTimeMs);
                } catch (Exception exc)
                {
                    Console.WriteLine(exc);
                }
            }
        }
    }
}