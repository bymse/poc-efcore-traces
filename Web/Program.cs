using DataLayer;
using DataLayer.EFCore;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Web;

using var observer = DataLayerConfiguration.StartObserver();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder
    .Services
    .AddDataLayer(builder.Configuration)
    .AddScoped<DataGenerator>()
    .AddOpenTelemetry()
    .UseOtlpExporter(OtlpExportProtocol.HttpProtobuf, new Uri("http://localhost:8080/otlp-http"))
    .ConfigureResource(resource => resource
        .AddService(serviceName: builder.Environment.ApplicationName))
    .WithTracing(e =>
    {
        e
            .SetSampler(new AlwaysOnSampler())
            .AddSource(DataLayerConfiguration.TELEMETRY_SOURCE)
            .AddAspNetCoreInstrumentation();
    });

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<IMigrator>();
    await dbContext.MigrateAsync();

    var dataGenerator = scope.ServiceProvider.GetRequiredService<DataGenerator>();
    await dataGenerator.GenerateOrders();
    await dataGenerator.GenerateOrdersForFirstCustomer();
}

app.Run();