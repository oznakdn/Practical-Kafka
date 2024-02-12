using CustomerService.API.MessageService;
using CustomerService.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Shared.Constraints;
using Services.Shared.Dtos;

namespace CustomerService.API.Controllers;

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
