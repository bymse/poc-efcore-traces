using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;

namespace DataLayer.Telemetry;

public static class DataLayerTelemetryConfiguration
{
    private static readonly ProxyGenerator Generator = new();

    public static EFCoreDiagnosticSourceObserver AddDataLayerTelemetry(this IServiceCollection services)
    {
        services.AddSingleton<RepositoryInterceptor>();

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
                    return Generator.CreateClassProxyWithTarget(instance, interceptor);
                });
            }
        }

        var observer = new EFCoreDiagnosticSourceObserver();
        observer.Subscribe();

        return observer;
    }
}