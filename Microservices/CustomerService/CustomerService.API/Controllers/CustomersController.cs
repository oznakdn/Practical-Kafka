using Confluent.Kafka;
using CustomerService.API.MessageService;
using CustomerService.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Shared.Constraints;
using Services.Shared.Dtos;
using System.Text.Json;

namespace CustomerService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController(CustomerDbContext context) : ControllerBase
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
        await producer.ProduceAsync("createdOrder", message);
        //await messageProducer.SendMessageAsync("createdOrder", new CustomerDto(customer.Id, customer.Name, customer.Surname, customer.Balance));
        return Ok();
    }

}
