using DataLayer.EFCore;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Repositories;

internal class OrdersRepository(MyDbContext dbContext) : IOrdersRepository
{
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

    public Task<int> Count()
    {
        return dbContext.Orders
            .CountAsync();
    }

    public Task CreateOrders(params Order[] order)
    {
        dbContext.Orders.AddRange(order);
        return dbContext.SaveChangesAsync();
    }
}