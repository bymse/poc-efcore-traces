using DataLayer;
using DataLayer.EFCore;
using DataLayer.Repositories;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

using var observer = builder.Services.AddDataLayer(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUi(options => { options.DocumentPath = "openapi/v1.json"; });
}

app.MapGet("orders", ([FromServices] IOrdersRepository repo) => repo.GetAll());

var migrator = app.Services.GetRequiredService<IMigrator>();
await migrator.MigrateAsync();

app.Run();