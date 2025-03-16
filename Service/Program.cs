using DataLayer;
using DataLayer.EFCore;
using DataLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder
    .Services
    .AddOpenTelemetry()
    .WithTracing(e =>
    {
        e.AddSource(DataLayerConfiguration.TelemetrySource);
        e.AddConsoleExporter();
    });

using var observer = builder.Services.AddDataLayer(builder.Configuration);

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