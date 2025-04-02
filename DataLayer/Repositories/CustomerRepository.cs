using DataLayer.EFCore;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Repositories;

internal class CustomerRepository(MyDbContext dbContext) : ICustomerRepository
{
    public Task<Customer[]> GetCustomers()
    {
        return dbContext.Customers.ToArrayAsync();
    }

    public Task<Customer[]> GetCustomers(int skip, int take)
    {
        return dbContext.Customers
            .OrderBy(e => e.Id)
            .Skip(skip)
            .Take(take).ToArrayAsync();
    }

    public Task<int> GetCustomersCount()
    {
        return dbContext.Customers.CountAsync();
    }

    public Task CreateCustomer(Customer customer)
    {
        dbContext.Customers.Add(customer);
        return dbContext.SaveChangesAsync();
    }

    public Task<Customer?> GetCustomerById(int customerId)
    {
        return dbContext.Customers.FirstOrDefaultAsync(c => c.Id == customerId);
    }

    public Task CreateCustomers(IEnumerable<Customer> customers)
    {
        dbContext.Customers.AddRange(customers);
        return dbContext.SaveChangesAsync();
    }
}