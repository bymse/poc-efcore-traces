using DataLayer.Entities;

namespace DataLayer.Repositories;

public interface IOrdersRepository
{
    Task<OrderModel[]> GetAllModels(int take);
    Task CreateOrders(params Order[] order);
    Task<int> GetCustomerOrdersCount(int customerId);
    Task<Order[]> GetCustomerOrders(int customerId, int skip, int take);
}

public class OrderModel
{
    public int OrderId { get; init; }
    public required string CustomerFirstName { get; init; }
    public required string CustomerLastName { get; init; }
    public required string ProductName { get; init; }
}