# Documentation for our backend

# Requirements

- Docker Installation

# Startup

- `docker-compose up -d` to start the environment

## SFR Kafka Exercise 1

### Analyze how the following things are related

- Number of brokers
- Number of partitons
- Number of Replicas
- in.sync.replica Configuration

You cannot specify a replication factor greater than the number of brokers you have

//Create new topic
kafka-topics.sh --bootstrap-server kafka:9092 --create --replication-factor 1 --partitions 4 --topic topic-name

//List all topics
kafka-topics.sh --bootstrap-server kafka:9092 --list

//Produce new topics
kafka-console-producer.sh --topic quickstart-events --bootstrap-server kafka:9092

//Read all events from the beginning
kafka-console-consumer.sh --topic quickstart-events --from-beginning --bootstrap-server kafka:9092
