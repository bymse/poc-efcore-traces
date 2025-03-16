using Microsoft.EntityFrameworkCore;

namespace DataLayer.EFCore;

internal class Migrator(MyDbContext dbContext) : IMigrator
{
    public async Task MigrateAsync()
    {
        await dbContext.Database.MigrateAsync();
    }
}

public interface IMigrator
{
    Task MigrateAsync();
}