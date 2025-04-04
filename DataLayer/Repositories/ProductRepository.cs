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

    public Task<Product[]> GetProducts(int skip, int take)
    {
        return dbContext.Products
            .OrderBy(p => p.Id)
            .Skip(skip)
            .Take(take)
            .ToArrayAsync();
    }

    public Task<int> GetProductsCount()
    {
        return dbContext.Products.CountAsync();
    }

    public Task<Product?> GetProductById(int id)
    {
        return dbContext.Products
            .Include(p => p.Orders)
            .ThenInclude(o => o.Customer)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public Task CrateProduct(Product product)
    {
        dbContext.Products.Add(product);
        return dbContext.SaveChangesAsync();
    }
}