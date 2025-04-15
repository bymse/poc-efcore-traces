using DataLayer;
using DataLayer.EFCore;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Web;

using var observer = DataLayerConfiguration.StartObserver();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder
    .Services
    .AddDataLayer(builder.Configuration)
    .AddOpenTelemetry()
    .UseOtlpExporter(OtlpExportProtocol.HttpProtobuf, new Uri(builder.Configuration.GetConnectionString("OtlpHttp")!))
    .ConfigureResource(resource => resource
        .AddService(serviceName: builder.Environment.ApplicationName))
    .WithTracing(e =>
    {
        e
            .SetSampler(new AlwaysOnSampler())
            .AddSource(DataLayerConfiguration.TELEMETRY_SOURCE)
            .AddAspNetCoreInstrumentation();
    })
    .WithMetrics(e =>
    {
        e
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
}

app.Run();