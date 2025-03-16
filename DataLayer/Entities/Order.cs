namespace DataLayer.Entities;

public class Customer
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required List<Order> Orders { get; set; }
}

public class Product
{
    public int Id { get; set; }
    
    public required string Name { get; set; }
    public required string Category { get; set; }
    
    public required List<Order> Orders { get; set; }
}

public class Order
{
    public int Id { get; set; }
    
    public required string Description { get; set; }
    
    public DateTimeOffset CreatedDate { get; set; }

    public int CustomerId { get; set; }
    public required Customer Customer { get; set; }
    
    public int ProductId { get; set; }
    public required Product Product { get; set; }
}