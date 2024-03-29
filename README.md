# Documentation for our backend

Repo: https://github.com/fh-hotel-booking/hotel-booking-backend

# Requirements

- Docker Installation

# Startup

- `docker-compose up -d` to start the environment

## SFR Kafka Exercise 1

Important: don't rename the folder after cloning. If you rename the folder, the scripts will break and you have to adjust the network name.

### Kafka CLI

Interact with the kafka instance through the `kafka-script.sh` ord `kafka-script.bat`.
It starts a docker container in the same network as the kafka container in the docker-compose environment and executes the kafka cli scripts.

#### Create Topic

`./kafka-script.bat kafka-topics.sh --bootstrap-server kafka1:9092 --create --replication-factor 2 --partitions 2 --topic topic-test`

#### List all Topics

`kafka-topics.sh --bootstrap-server kafka1:9092 --list`

#### Start a CLI producer

Execute
`./kafka-script.bat kafka-console-producer.sh --topic topic-test --bootstrap-server kafka1:9092`
and then you start writing a text message and with enter you send an event to the topic `topic-test`.
Exit with Strg+C

#### Start a CLI consumer

Execute
`./kafka-script.bat kafka-console-consumer.sh --topic topic-test --from-beginning --bootstrap-server kafka1:9092`
and then you should see all the events in the topic. (Execute the command in a seperate terminal from the producer and you can see new events arriving in realtime)

### Analyze how the following things are related

- Number of brokers
  A Kafka client communicates with the Kafka brokers via the network for writing (or reading) events. Once received, the brokers will store the events in a durable and fault-tolerant manner for as long as you need—even forever.
- Number of partitons
  Topics are split into partitions accross brokers to help scaling.
- Number of Replicas
  The amount of times the partitions are replicated on different brokers.
  This is the reason why you cannot specify a replication factor greater than the number of brokers you have

- in.sync.replica Configuration
  Kafka gives a producer confirmation when it writes produces an event for a topic. `in.sync.replica` indicates the amount of replicas that need to send an acknowledgment before the produces is notified of a success.

https://kafka.apache.org/quickstart

You cannot specify a replication factor greater than the number of brokers you have

## SFR Kafka Exercise 2

How is the schema validated based on your selected compatibility mode.

We use the confluent default compatibility mode "Backwards". That means consumer using Schema X can process data produced with schema X or X-1, but they can't use schema X-2 or earlier.
Source: https://docs.confluent.io/platform/current/schema-registry/fundamentals/schema-evolution.html

# Developers

- Lukas Nowy
- Jenny Gebauer
- Florian Brunner
