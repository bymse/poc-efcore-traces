using DataLayer.Entities;
using DataLayer.Repositories;

namespace Service.Application.Generator;

public class DataGenerator(
    ICustomerRepository customerRepository,
    IOrdersRepository ordersRepository,
    IProductRepository productRepository)
{
    public async Task GenerateOrders()
    {
        var existingCustomers = await customerRepository.GetCustomers();
        var existingProducts = await productRepository.GetProducts();

        const int customerCount = 1000;
        const int productCount = 100;

        if (existingCustomers.Length >= customerCount && existingProducts.Length >= productCount)
        {
            return;
        }

        var newCustomers = new List<Customer>();
        for (int i = existingCustomers.Length; i < customerCount; i++)
        {
            newCustomers.Add(new Customer
            {
                FirstName = $"Customer{i}",
                LastName = $"LastName{i}",
                Orders = new List<Order>()
            });
        }

        if (newCustomers.Count > 0)
        {
            await customerRepository.CreateCustomers(newCustomers);
        }

        var newProducts = new List<Product>();
        for (int i = existingProducts.Length; i < productCount; i++)
        {
            newProducts.Add(new Product
            {
                Name = $"Product{i}",
                Category = $"Category{i % 10}",
                Orders = new List<Order>()
            });
        }

        foreach (var product in newProducts)
        {
            await productRepository.CrateProduct(product);
        }

        var allCustomers = await customerRepository.GetCustomers();
        var allProducts = await productRepository.GetProducts();

        var random = new Random(42);
        var orders = new List<Order>();

        foreach (var customer in allCustomers.Take(customerCount))
        {
            for (int i = 0; i < 2; i++)
            {
                var product = allProducts[random.Next(allProducts.Length)];
                orders.Add(new Order
                {
                    Description = $"Order for {customer.FirstName} #{i}",
                    CreatedDate = DateTimeOffset.UtcNow,
                    CustomerId = customer.Id,
                    ProductId = product.Id,
                    Customer = customer,
                    Product = product
                });
            }
        }

        await ordersRepository.CreateOrders([.. orders]);
    }

    public async Task GenerateOrdersForFirstCustomer()
    {
        const int customerId = 1;

        var customer = await customerRepository.GetCustomerById(customerId) ?? throw new Exception($"Customer with ID {customerId} not found.");

        var products = await productRepository.GetProducts();

        if (products.Length == 0)
        {
            var defaultProduct = new Product { Name = "Default Product", Category = "Misc", Orders = [] };
            await productRepository.CrateProduct(defaultProduct);
            products = [defaultProduct];
        }

        const int totalOrders = 5000;
        var orders = new List<Order>(totalOrders);

        for (int i = 0; i < totalOrders; i++)
        {
            var product = products[i % products.Length];

            orders.Add(new Order
            {
                Description = $"Multi-Product Order #{i}",
                CreatedDate = DateTimeOffset.UtcNow,
                CustomerId = customerId,
                ProductId = product.Id,
                Customer = customer,
                Product = product
            });
        }

        await ordersRepository.CreateOrders([.. orders]);
    }
}