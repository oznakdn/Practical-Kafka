
![kafka](https://github.com/oznakdn/Practical-Kafka/assets/79724084/95f090be-300b-4b68-a84b-3c291c59b2da)


## Nuget package
```nuget
Confluent.Kafka
```

## docker-compose

```docker-compose
version: '3.8'

services:
  kafka:
    image: confluentinc/cp-kafka:6.0.14
    depends_on:
      - zookeeper
    ports:
      - '29092:29092'
    environment:
      KAFKA_BROKER_ID: 1
      KAFKA_ZOOKEEPER_CONNECT: 'zookeeper:2181'
      KAFKA_ADVERTISED_LISTENERS: LISTENER_DOCKER_INTERNAL://kafka:9092,LISTENER_DOCKER_EXTERNAL://${DOCKER_HOST_IP:-127.0.0.1}:29092
      KAFKA_LISTENER_SECURITY_PROTOCOL_MAP: LISTENER_DOCKER_INTERNAL:PLAINTEXT,LISTENER_DOCKER_EXTERNAL:PLAINTEXT
      KAFKA_INTER_BROKER_LISTENER_NAME: LISTENER_DOCKER_INTERNAL
      KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR: 1

  kafka-ui:
    image: provectuslabs/kafka-ui:latest
    ports:
      - 8085:8080
    environment:
      KAFKA_CLUSTERS_0_NAME: local
      KAFKA_CLUSTERS_0_BOOTSTRAPSERVERS: kafka:9092
      DYNAMIC_CONFIG_ENABLED: 'true'

  zookeeper:
    image: confluentinc/cp-zookeeper:6.0.14
    ports:
      - '22181:2181'
    environment:
      ZOOKEEPER_CLIENT_PORT: 2181
      ZOOKEEPER_TICK_TIME: 2000
```

## Kafka UI

![Screenshot_1](https://github.com/oznakdn/Practical-Kafka/assets/79724084/332530c5-81c0-4fc6-b5b6-0dd2e93a7230)
