using DataLayer.EFCore;
using DataLayer.Telemetry;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddDbContext<MyAppDbContext>()
    .AddOpenApi();

using var observer = new EFCoreDiagnosticSourceObserver();
observer.Subscribe();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUi(options => { options.DocumentPath = "openapi/v1.json"; });
}

app.Run();