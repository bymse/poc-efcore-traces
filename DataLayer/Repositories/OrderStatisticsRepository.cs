using DataLayer.EFCore;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Repositories
{
    internal class OrderStatisticsRepository(MyDbContext dbContext) : IOrderStatisticsRepository
    {
        public async Task<int> GetOrderStatisticsCount()
        {
            return await dbContext.Orders.CountAsync();
        }

        public async Task<OrderStatistics[]> GetPaginatedOrderStatistics(int skip, int take)
        {
            var query = from order in dbContext.Orders
                        select new OrderStatistics
                        {
                            OrderId = order.Id,
                            CustomerId = order.CustomerId,
                            ProductId = order.ProductId,
                            Description = order.Description,
                            CustomerFullName = order.Customer.FirstName + " " + order.Customer.LastName,
                            ProductName = order.Product.Name,
                            TotalOrdersForCustomer = dbContext.Orders.Count(o => o.CustomerId == order.CustomerId)
                        };

            query = query.OrderBy(o => o.OrderId);

            return await query.Skip(skip).Take(take).ToArrayAsync();
        }

        public async Task<ProductPopularity> GetMostPopularProduct()
        {
            return await dbContext.Orders
                .GroupBy(o => o.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    OrderCount = g.Count()
                })
                .OrderByDescending(x => x.OrderCount)
                .Take(1)
                .Join(
                    dbContext.Products,
                    orderGroup => orderGroup.ProductId,
                    product => product.Id,
                    (orderGroup, product) => new ProductPopularity
                    {
                        ProductId = product.Id,
                        Name = product.Name,
                        Category = product.Category,
                        OrderCount = orderGroup.OrderCount
                    })
                .FirstAsync();
        }

        public async Task<CustomerOrderCount> GetCustomerWithMostOrders()
        {
            return await dbContext.Orders
                .GroupBy(o => o.CustomerId)
                .Select(g => new { CustomerId = g.Key, OrderCount = g.Count() })
                .OrderByDescending(x => x.OrderCount)
                .Take(1)
                .Join(
                    dbContext.Customers,
                    orderGroup => orderGroup.CustomerId,
                    customer => customer.Id,
                    (orderGroup, customer) => new CustomerOrderCount
                    {
                        CustomerId = customer.Id,
                        FullName = $"{customer.FirstName} {customer.LastName}",
                        OrderCount = orderGroup.OrderCount
                    })
                .FirstAsync();
        }
        public async Task<OrderAveragesByCategory> GetOrderAveragesByProductCategory()
        {
            var ordersByCategory = await dbContext.Orders
                .Join(
                    dbContext.Products,
                    order => order.ProductId,
                    product => product.Id,
                    (order, product) => new { order.CustomerId, product.Category, order.ProductId })
                .GroupBy(x => x.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalOrders = g.Count(),
                    UniqueProducts = g.Select(o => o.ProductId).Distinct().Count(),
                    UniqueCustomers = g.Select(o => o.CustomerId).Distinct().Count(),
                })
                .Select(x => new OrderAveragesByCategory
                {
                    Category = x.Category,
                    TotalOrders = x.TotalOrders,
                    UniqueProducts = x.UniqueProducts,
                    UniqueCustomers = x.UniqueCustomers,
                })
                .OrderByDescending(c => c.TotalOrders)
                .FirstAsync();

            return ordersByCategory;
        }
    }
}