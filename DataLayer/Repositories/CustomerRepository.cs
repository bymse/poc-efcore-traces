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

    public Task CreateCustomer(Customer customer)
    {
        dbContext.Customers.Add(customer);
        return dbContext.SaveChangesAsync();
    }
}