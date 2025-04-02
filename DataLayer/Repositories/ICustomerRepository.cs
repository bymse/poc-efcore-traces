using DataLayer.Entities;

namespace DataLayer.Repositories;

public interface ICustomerRepository
{
    Task<Customer[]> GetCustomers();
    Task<Customer[]> GetCustomers(int skip, int take);
    Task<int> GetCustomersCount();
    Task CreateCustomer(Customer customer);
    Task<Customer?> GetCustomerById(int customerId);
    Task CreateCustomers(IEnumerable<Customer> customers);
}