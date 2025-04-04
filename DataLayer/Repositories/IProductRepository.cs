using DataLayer.Entities;

namespace DataLayer.Repositories;

public interface IProductRepository
{
    Task<Product[]> GetProducts();
    Task<Product[]> GetProducts(int skip, int take);
    Task<int> GetProductsCount();
    Task<Product?> GetProductById(int id);
    Task CrateProduct(Product product);
}