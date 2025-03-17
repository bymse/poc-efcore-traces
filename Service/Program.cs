using DataLayer;
using DataLayer.EFCore;
using DataLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using var observer = DataLayerConfiguration.StartObserver();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder
    .Services
    .AddDataLayer(builder.Configuration)
    .AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName: builder.Environment.ApplicationName))
    .WithTracing(e => { e
        .SetSampler(new AlwaysOnSampler())
        .AddSource(DataLayerConfiguration.TELEMETRY_SOURCE)
        .AddConsoleExporter(); });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUi(options => { options.DocumentPath = "openapi/v1.json"; });
}

app.MapGet("orders", ([FromServices] IOrdersRepository repo) => repo.GetAll());

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<IMigrator>();
    await dbContext.MigrateAsync();
}


app.Run();