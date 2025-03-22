using DataLayer.Entities;

namespace DataLayer.Repositories;

public interface IProductRepository
{
    Task<Product[]> GetProducts();
    Task CrateProduct(Product product);
}