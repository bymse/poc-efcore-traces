namespace DataLayer.Repositories
{
    public interface IOrderStatisticsRepository
    {
        Task<int> GetOrderStatisticsCount();
        Task<OrderStatistics[]> GetPaginatedOrderStatistics(int skip, int take);
        Task<ProductPopularity> GetMostPopularProduct();
        Task<CustomerOrderCount> GetCustomerWithMostOrders();
        Task<OrderAveragesByCategory> GetOrderAveragesByProductCategory();
        Task<CategoryPopularity> GetMostPopularCategoryForCustomer(int customerId);
    }
    public class OrderStatistics
    {
        public int OrderId { get; set; }
        public string Description { get; set; }
        public int CustomerId { get; set; }
        public string CustomerFullName { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int TotalOrdersForCustomer { get; set; }
    }

    public class ProductPopularity
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int OrderCount { get; set; }
    }

    public class CustomerOrderCount
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public int OrderCount { get; set; }
    }
    public class OrderAveragesByCategory
    {
        public string Category { get; set; }
        public int TotalOrders { get; set; }
        public int UniqueProducts { get; set; }
        public int UniqueCustomers { get; set; }
    }
    public class CategoryPopularity
    {
        public string Category { get; set; }
        public int OrderCount { get; set; }
    }
}
