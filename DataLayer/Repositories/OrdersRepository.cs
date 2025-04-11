using DataLayer.EFCore;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Repositories;

internal class OrdersRepository(MyDbContext dbContext) : IOrdersRepository
{
    public Task<int> GetCustomerOrdersCount(int customerId)
    {
        return dbContext.Orders
            .Where(o => o.CustomerId == customerId)
            .CountAsync();
    }

    public Task<Order[]> GetCustomerOrders(int customerId, int skip, int take)
    {
        return dbContext.Orders
            .Where(o => o.CustomerId == customerId)
            .Include(o => o.Product)
            .OrderBy(e => e.Id)
            .Skip(skip)
            .Take(take)
            .ToArrayAsync();
    }

    public Task<OrderModel[]> GetAllModels(int take)
    {
        return dbContext.Orders
            .OrderBy(e => e.CreatedDate)
            .Take(take)
            .Select(e => new OrderModel
            {
                ProductName = e.Product.Name,
                CustomerFirstName = e.Customer.FirstName,
                CustomerLastName = e.Customer.LastName,
                OrderId = e.Id
            })
            .ToArrayAsync();
    }

    public Task CreateOrders(params Order[] order)
    {
        dbContext.Orders.AddRange(order);
        return dbContext.SaveChangesAsync();
    }
}