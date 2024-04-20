package at.technikum.hotelbookingtransformer;

import at.technikum.hotelbooking.avro.AverageHotelPrice;
import at.technikum.hotelbooking.avro.Booking;
import io.confluent.kafka.serializers.KafkaAvroDeserializer;
import io.confluent.kafka.serializers.KafkaAvroDeserializerConfig;
import io.confluent.kafka.streams.serdes.avro.SpecificAvroSerde;
import org.apache.kafka.clients.consumer.ConsumerConfig;
import org.apache.kafka.common.serialization.Serde;
import org.apache.kafka.common.serialization.Serdes;
import org.apache.kafka.common.serialization.StringDeserializer;
import org.apache.kafka.streams.KafkaStreams;
import org.apache.kafka.streams.StreamsBuilder;
import org.apache.kafka.streams.StreamsConfig;
import org.apache.kafka.streams.Topology;
import org.apache.kafka.streams.kstream.Consumed;
import org.apache.kafka.streams.kstream.Materialized;
import org.apache.kafka.streams.kstream.Produced;
import org.checkerframework.checker.units.qual.A;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Map;
import java.util.Properties;
import java.util.concurrent.CountDownLatch;

public class Main {

    protected static final Logger logger = LoggerFactory.getLogger(Main.class);
    static final String ENV_INPUT_CONSUMER_TOPIC_NAME = "INPUT_CONSUMER_TOPIC_NAME";
    static final String ENV_INPUT_CONSUMER_KAFKA_BOOTSTRAP_SERVER = "INPUT_CONSUMER_KAFKA_BOOTSTRAP_SERVER";
    static final String ENV_OUTPUT_PRODUCER_KAFKA_BOOTSTRAP_SERVER = "OUTPUT_PRODUCER_KAFKA_BOOTSTRAP_SERVER";
    static final String ENV_OUTPUT_PRODUCER_TOPIC_NAME = "OUTPUT_PRODUCER_TOPIC_NAME";
    static final String ENV_TRANSFORMER_KAFKA_SCHEMA_SERVER = "TRANSFORMER_KAFKA_SCHEMA_SERVER";
    static final String ENV_INPUT_CONSUMER_GROUP_ID = "INPUT_CONSUMER_GROUP_ID";

    public static void main(String[] args) {

        final String inputTopic = System.getenv(ENV_INPUT_CONSUMER_TOPIC_NAME);;
        final String outputTopic = System.getenv(ENV_OUTPUT_PRODUCER_TOPIC_NAME);;
        String inputKafkaServerConfig = System.getenv(ENV_INPUT_CONSUMER_KAFKA_BOOTSTRAP_SERVER);
        String consumerGroupId = System.getenv(ENV_INPUT_CONSUMER_GROUP_ID);
        String schemaServerConfig = System.getenv(ENV_TRANSFORMER_KAFKA_SCHEMA_SERVER);
        final Properties streamProperties = new Properties();
        streamProperties.put(StreamsConfig.APPLICATION_ID_CONFIG, "booking-data-transformer");
        streamProperties.put(StreamsConfig.BOOTSTRAP_SERVERS_CONFIG, inputKafkaServerConfig);
        streamProperties.put(ConsumerConfig.GROUP_ID_CONFIG, consumerGroupId);

        streamProperties.put(StreamsConfig.DEFAULT_KEY_SERDE_CLASS_CONFIG, Serdes.StringSerde.class);
        streamProperties.put(StreamsConfig.DEFAULT_VALUE_SERDE_CLASS_CONFIG, SpecificAvroSerde.class);

        streamProperties.put(ConsumerConfig.KEY_DESERIALIZER_CLASS_CONFIG, StringDeserializer.class);
        streamProperties.put(ConsumerConfig.VALUE_DESERIALIZER_CLASS_CONFIG, KafkaAvroDeserializer.class);

        streamProperties.put(ConsumerConfig.AUTO_OFFSET_RESET_CONFIG, "earliest");

        streamProperties.put(KafkaAvroDeserializerConfig.SPECIFIC_AVRO_READER_CONFIG, true);
        streamProperties.put(KafkaAvroDeserializerConfig.SCHEMA_REGISTRY_URL_CONFIG, schemaServerConfig);
        final Map<String, String> serdeConfig = Collections.singletonMap(KafkaAvroDeserializerConfig.SCHEMA_REGISTRY_URL_CONFIG, schemaServerConfig);

        final StreamsBuilder streamsBuilder = new StreamsBuilder();
        final Serde<String> keyGenericAvroSerde = new Serdes.StringSerde();
        keyGenericAvroSerde.configure(serdeConfig, true);
        final Serde<Booking> bookingDataAvroSerde = new SpecificAvroSerde<>();
        bookingDataAvroSerde.configure(serdeConfig, false);

        logger.info("Kafka Server Config: " + inputKafkaServerConfig);
        logger.info("Schema Server Config: " + schemaServerConfig);
        logger.info("Input Topic: " + inputTopic);
        logger.info("Output Topic: " + outputTopic);

        streamsBuilder.stream(inputTopic, Consumed.with(keyGenericAvroSerde, bookingDataAvroSerde))
                .groupBy((key, value) -> value.getHotelName())
                .aggregate(Main::getAverageHotelPriceAggregator, (key, value, aggregator) -> {
                    aggregator.setHotelName(key);
                    aggregator.getPrices().add(value.getPrice());
                    return aggregator;
                }, Materialized.as("TOPIC_AVERAGE_HOTEL_PRICE")
                )
                .toStream()
                .mapValues((key, value) -> value.getPrices().stream().mapToDouble(x -> x).sum() / value.getPrices().size())
                .to(outputTopic, Produced.with(Serdes.String(), Serdes.Double()));

        final Topology topology = streamsBuilder.build();
        final KafkaStreams streams = new KafkaStreams(topology, streamProperties);
        final CountDownLatch latch = new CountDownLatch(1);

        attachStreamToCountDownLatch(streams, latch);
    }

    private static AverageHotelPrice getAverageHotelPriceAggregator(){
        AverageHotelPrice averageHotelPrice = new AverageHotelPrice();
        averageHotelPrice.setPrices(new ArrayList<>());
        return averageHotelPrice;
    }

    private static void attachStreamToCountDownLatch(KafkaStreams streams, CountDownLatch latch) {
        Runtime.getRuntime().addShutdownHook(new Thread("streams-shutdown-hook") {
            @Override
            public void run() {
                streams.close();
                latch.countDown();
            }
        });

        try {
            streams.start();
            latch.await();
        } catch (Exception e) {
            System.exit(1);
        }
        System.exit(0);
    }
}