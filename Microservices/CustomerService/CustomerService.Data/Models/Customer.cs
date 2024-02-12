namespace CustomerService.Data.Models;

public class Customer
{
    public Customer(string name, string surname, decimal balance)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
        Surname = surname;
        Balance = balance;
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public decimal Balance { get; set; }

}
