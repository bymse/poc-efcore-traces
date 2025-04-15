using DataGenerator;
using DataLayer;
using DataLayer.EFCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(e => { e.AddJsonFile("appsettings.json"); })
    .ConfigureServices((e, services) =>
    {
        services.AddDataLayer(e.Configuration);
        services.AddTransient<Generator>();
    })
    .Build();

Console.WriteLine("Starting data generation...");

using var scope = host.Services.CreateScope();
var serviceProvider = scope.ServiceProvider;

var migrator = serviceProvider.GetRequiredService<IMigrator>();
await migrator.MigrateAsync();

Console.WriteLine("Generating initial data...");
var dataGenerator = serviceProvider.GetRequiredService<Generator>();
await dataGenerator.GenerateOrders();
Console.WriteLine("Initial data generation completed.");

Console.WriteLine("Generating orders for first customer...");
await dataGenerator.GenerateOrdersForFirstCustomer();
Console.WriteLine("Orders generation completed.");