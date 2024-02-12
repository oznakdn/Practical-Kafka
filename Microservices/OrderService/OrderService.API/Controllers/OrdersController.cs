using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data.Context;

namespace OrderService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(OrderDbContext context) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        return Ok(await context.Orders.ToListAsync());
    }

    [HttpGet("{customerId}")]
    public async Task<IActionResult> GetCustomerOrders(string customerId)
    {
        return Ok(await context.Orders.Where(_ => _.CustomerId == customerId).ToListAsync());
    }

}
