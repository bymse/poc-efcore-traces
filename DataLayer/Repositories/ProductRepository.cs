using DataLayer.EFCore;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Repositories;

internal class ProductRepository(MyDbContext dbContext) : IProductRepository
{
    public Task<Product[]> GetProducts()
    {
        return dbContext.Products.ToArrayAsync();
    }

    public Task CrateProduct(Product product)
    {
        dbContext.Products.Add(product);
        return dbContext.SaveChangesAsync();
    }
}