using DataLayer.EFCore;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Repositories;

internal class OrdersRepository(MyDbContext dbContext) : IOrdersRepository
{
    public Task<OrderModel[]> GetAll()
    {
        return dbContext.Orders
            .Select(e => new OrderModel())
            .ToArrayAsync();
    }

    public Task<OrderModel?> Find(int id)
    {
        return dbContext.Orders
            .Include(e => e.Customer)
            .Include(e => e.Product)
            .Where(e => e.Id == id)
            .Select(e => new OrderModel())
            .FirstOrDefaultAsync();
    }
}

public class OrderModel
{
}