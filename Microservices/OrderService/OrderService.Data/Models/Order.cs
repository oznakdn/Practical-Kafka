namespace OrderService.Data.Models;

public class Order
{
    public Order(string customerId, DateTime dateOfOrder)
    {
        Id = Guid.NewGuid().ToString();
        CustomerId = customerId;
        DateOfOrder = dateOfOrder;
    }

    public string Id { get; set; }
    public string CustomerId { get; set; }
    public DateTime DateOfOrder { get; set; }

}
