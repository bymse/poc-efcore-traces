using DataLayer;
using DataLayer.EFCore;
using DataLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Service.Application;
using Service.Application.Generator;

using var observer = DataLayerConfiguration.StartObserver();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
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

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUi(options => { options.DocumentPath = "openapi/v1.json"; });
}

app.MapGet("orders", ([FromQuery] int take, [FromServices] IOrdersRepository repo) => repo.GetAllModels(take));

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<IMigrator>();
    await dbContext.MigrateAsync();

    var dataGenerator = scope.ServiceProvider.GetRequiredService<DataGenerator>();
    await dataGenerator.GenerateOrders();
    await dataGenerator.GenerateOrdersForFirstCustomer();
}

app.Run();