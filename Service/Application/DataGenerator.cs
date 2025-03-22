using DataLayer.Entities;
using DataLayer.Repositories;

namespace Service.Application;

public class DataGenerator(
    ICustomerRepository customerRepository,
    IOrdersRepository ordersRepository,
    IProductRepository productRepository)
{
    public async Task Generate()
    {
        var customers = await customerRepository.GetCustomers();
        var customer = customers.FirstOrDefault();
        if (customer == null)
        {
            customer = new Customer { FirstName = "Default", LastName = "Customer", Orders = new List<Order>() };
            await customerRepository.CreateCustomer(customer);
        }

        var products = await productRepository.GetProducts();
        var product = products.FirstOrDefault();
        if (product == null)
        {
            product = new Product { Name = "Default Product", Category = "Misc", Orders = new List<Order>() };
            await productRepository.CrateProduct(product);
        }
        
        var count = await ordersRepository.Count();
        if (count >= 10_000)
        {
            return;
        }

        var orders = Enumerable.Range(count, 10_000).Select(i => new Order
        {
            Description = $"Order #{i}",
            CreatedDate = DateTimeOffset.UtcNow,
            CustomerId = customer.Id,
            ProductId = product.Id,
            Customer = customer,
            Product = product
        }).ToArray();

        await ordersRepository.CreateOrders(orders);
    }
}