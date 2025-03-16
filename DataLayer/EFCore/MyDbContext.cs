using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.EFCore;

public class MyAppDbContext(DbContextOptions<MyAppDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired();
            entity.Property(e => e.LastName).IsRequired();

            entity.HasMany(e => e.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId)
                .IsRequired();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(100);

            entity.HasMany(p => p.Orders)
                .WithOne(o => o.Product)
                .HasForeignKey(o => o.ProductId)
                .IsRequired();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.CreatedDate).IsRequired();
        });
    }
}