using DataLayer.Entities;

namespace DataLayer.Repositories;

public interface ICustomerRepository
{
    Task<Customer[]> GetCustomers();
    Task CreateCustomer(Customer customer);
}