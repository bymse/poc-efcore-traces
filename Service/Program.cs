using DataLayer.EFCore;
using DataLayer.Repositories;
using DataLayer.Telemetry;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddDbContext<MyAppDbContext>(e =>
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        e.UseNpgsql(connectionString);
    })
    .AddOpenApi();

using var observer = builder.Services.AddDataLayerTelemetry();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUi(options => { options.DocumentPath = "openapi/v1.json"; });
}

app.MapGet("orders", ([FromServices] IOrdersRepository repo) => repo.GetAll());

app.Run();