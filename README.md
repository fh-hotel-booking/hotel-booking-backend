# Documentation for our backend

# Requirements

- Docker Installation

# Startup

- `docker-compose up -d` to start the environment

## SFR Kafka Exercise 1

### Create Topic

Use the `kafka-script.bat` or `kafka-script.sh` script.
`./kafka-script.bat kafka-topics.sh --bootstrap-server kafka:9092 --create --replication-factor 1 --partitions 1 --topic topic-name`
It starts a docker container in the same network as the kafka container in the docker-compose environment and executes the script.

### Start a CLI producer

`./kafka-script.bat kafka-console-producer.sh --topic topic-test --bootstrap-server kafka:9092`

### Start a CLI consumer

`./kafka-script.bat kafka-console-consumer.sh --topic topic-test --from-beginning --bootstrap-server kafka:9092`

### Analyze how the following things are related

- Number of brokers
- Number of partitons
Topics are split into partitions accross brokers to help scaling.
- Number of Replicas
The amount of times the partitions are replicated on different brokers.
This is the reason why you cannot specify a replication factor greater than the number of brokers you have

- in.sync.replica Configuration
Kafka gives a producer confirmation when it writes produces an event for a topic. `in.sync.replica` indicates the amount of replicas that need to send an acknowledgment before the produces is notified of a success.

https://kafka.apache.org/quickstart


