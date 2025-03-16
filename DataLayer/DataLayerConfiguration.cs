using Castle.DynamicProxy;
using DataLayer.EFCore;
using DataLayer.Telemetry;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataLayer;

public static class DataLayerConfiguration
{
    public const string TELEMETRY_SOURCE = EFCoreDiagnosticEventsObserver.ACTIVITY_SOURCE_NAME;
    
    private static readonly ProxyGenerator Generator = new();

    public static IServiceCollection AddDataLayer(this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddTransient<IMigrator, Migrator>()
            .AddDbContext<MyDbContext>(e =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                e.UseNpgsql(connectionString);
            })
            .AddSingleton<RepositoryInterceptor>();

        foreach (var type in typeof(RepositoryInterceptor).Assembly.GetTypes()
                     .Where(e => e.IsClass)
                     .Where(e => !e.IsAbstract)
                     .Where(e => e.Name.EndsWith("Repository")))
        {
            foreach (var @interface in type.GetInterfaces())
            {
                services.AddScoped(type);
                services.AddScoped(@interface, sp =>
                {
                    var instance = sp.GetRequiredService(type);
                    var interceptor = sp.GetRequiredService<RepositoryInterceptor>();
                    return Generator.CreateInterfaceProxyWithTarget(@interface, Type.EmptyTypes, instance, interceptor);
                });
            }
        }

        return services;
    }

    public static IDisposable StartObserver()
    {
        var observer = new EFCoreDiagnosticSourceObserver();
        observer.Subscribe();

        return observer;
    }
}