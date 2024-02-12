using Confluent.Kafka;
using OrderService.Data.Context;
using OrderService.Data.Models;
using Services.Shared.Constraints;
using Services.Shared.Dtos;
using System.Text.Json;

namespace OrderService.MessageWorker
{
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
            consumer.Subscribe("createdOrder");

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
}
