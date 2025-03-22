using DataLayer.Entities;

namespace DataLayer.Repositories;

public interface IOrdersRepository
{
    Task<OrderModel[]> GetAllModels(int take);
    Task<int> Count();
    
    Task CreateOrders(params Order[] order);
}

public class OrderModel
{
    public int OrderId { get; init; }
    public required string CustomerFirstName { get; init; }
    public required string CustomerLastName { get; init; }
    public required string ProductName { get; init; }
}