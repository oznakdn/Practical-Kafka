
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

# Producer (Publisher)

### Message Service

```csharp
public class MessageProducerService
{
    public async Task SendMessageAsync(string topic, CustomerDto customer)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:29092",
            ClientId = "OrderClient",
            Acks = Acks.All
        };

        var message = new Message<string, string>
        {
            Key = customer.Id,
            Value = JsonSerializer.Serialize(customer)
        };

        using var producer = new ProducerBuilder<string, string>(config).Build();
        await producer.ProduceAsync(topic, message);
    }
}

```

### Api Controller
```csharp
[Route("api/[controller]")]
[ApiController]
public class CustomersController(CustomerDbContext context, MessageProducerService producerService) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult>GetCustomers()
    {
        return Ok(await context.Customers.ToListAsync());
    }


    [HttpGet("{customerId}")]
    public async Task<IActionResult>CreateCustomerOrder(string customerId)
    {
        var customer = await context.Customers.SingleOrDefaultAsync(_ => _.Id == customerId);
        if (customer is null) return NotFound();

        await producerService.SendMessageAsync(MessageTopic.CREATE_ORDER, new CustomerDto(customer.Id, customer.Name, customer.Surname, customer.Balance));
        return Ok();
    }
}

```

# Consumer (Subscriber)

### Worker Service
```csharp
public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly OrderDbContext _context;

        public Worker(ILogger<Worker> logger, OrderDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:29092",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                ClientId = "OrderClient",
                GroupId = "OrderGroup"
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe(MessageTopic.CREATE_ORDER);
            _logger.LogInformation("Connected kafka");

            while (!stoppingToken.IsCancellationRequested)
            {
                var data = consumer.Consume();

                if (data is not null)
                {
                    var customer = JsonSerializer.Deserialize<CustomerDto>(data.Message.Value);

                    if (customer!.Balance > 0)
                    {
                        _context.Orders.Add(new Order(customer.Id, DateTime.Now));
                        await _context.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation($"{customer.Id} Customer's order has been created successfully.");
                    }
                    else
                    {
                        _logger.LogInformation($"{customer.Id} Customer's balance is less than 0!");

                    }
                }

                _logger.LogInformation($"There is no message");
            }
        }
    }

```

## Kafka UI

![Screenshot_1](https://github.com/oznakdn/Practical-Kafka/assets/79724084/332530c5-81c0-4fc6-b5b6-0dd2e93a7230)
