using CustomerService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Data.Context;

public class CustomerDbContext : DbContext
{
    public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base(options)
    {

    }

    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .HasData(new Customer("John", "Doe", 1_000m));
    }
}
