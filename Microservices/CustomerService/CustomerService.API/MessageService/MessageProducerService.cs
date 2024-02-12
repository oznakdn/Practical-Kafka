using Confluent.Kafka;
using Services.Shared.Dtos;
using System.Text.Json;

namespace CustomerService.API.MessageService;

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
