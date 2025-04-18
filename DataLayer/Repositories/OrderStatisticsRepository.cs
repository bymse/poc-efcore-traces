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
            return await dbContext.Orders
                .Select(order => new OrderStatistics
                {
                    OrderId = order.Id,
                    CustomerId = order.CustomerId,
                    ProductId = order.ProductId,
                    Description = order.Description,
                    CustomerFullName = order.Customer.FirstName + " " + order.Customer.LastName,
                    ProductName = order.Product.Name,
                    TotalOrdersForCustomer = dbContext.Orders.Count(o => o.CustomerId == order.CustomerId)
                })
                .OrderBy(o => o.OrderId)
                .Skip(skip).Take(take).ToArrayAsync();
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

        public async Task<CustomerOrderStat> GetCustomerOrderStats(int customerId)
        {
            return await dbContext.Orders
                .GroupBy(e => e.CustomerId)
                .Select(g => new
                {
                    CustomerId = g.Key,
                    MostPopularCategory = g
                        .GroupBy(o => o.Product.Category)
                        .Select(cg => new
                        {
                            Category = cg.Key,
                            OrderCount = cg.Count()
                        })
                        .OrderByDescending(c => c.OrderCount)
                        .First().Category,

                    MostPopularCategoryOrdersCount = g
                        .GroupBy(o => o.Product.Category)
                        .Select(cg => new
                        {
                            Category = cg.Key,
                            OrderCount = cg.Count()
                        })
                        .OrderByDescending(c => c.OrderCount)
                        .First().OrderCount,

                    MostPopularProduct = g
                        .GroupBy(o => o.Product.Name)
                        .Select(pg => new
                        {
                            ProductName = pg.Key,
                            OrderCount = pg.Count()
                        })
                        .OrderByDescending(p => p.OrderCount)
                        .First().ProductName,

                    MostPopularProductOrdersCount = g
                        .GroupBy(o => o.Product.Name)
                        .Select(pg => new
                        {
                            ProductName = pg.Key,
                            OrderCount = pg.Count()
                        })
                        .OrderByDescending(p => p.OrderCount)
                        .First().OrderCount,
                })
                .Where(e => e.CustomerId == customerId)
                .Select(x => new CustomerOrderStat
                {
                    MostPopularCategory = x.MostPopularCategory,
                    MostPopularCategoryOrdersCount = x.MostPopularCategoryOrdersCount,
                    MostPopularProduct = x.MostPopularProduct,
                    MostPopularProductOrdersCount = x.MostPopularProductOrdersCount
                })
                .SingleAsync();
        }
    }
}