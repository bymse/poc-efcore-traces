using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataLayer.EFCore;

internal class MyDbContextFactory : IDesignTimeDbContextFactory<MyDbContext>
{
    public MyDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<MyDbContext>()
            .UseNpgsql();

        return new MyDbContext(builder.Options);
    }
}