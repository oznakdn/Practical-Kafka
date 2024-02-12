using Microsoft.EntityFrameworkCore;
using OrderService.Data.Models;

namespace OrderService.Data.Context;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext>options) : base(options)
    {
        
    }

    public DbSet<Order>Orders { get; set; }

}
