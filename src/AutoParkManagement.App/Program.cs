using System.Reflection;
using AutoParkManagement.Common;
using AutoParkManagement.Common.Database;
using AutoParkManagement.EndpointManagers;
using AutoParkManagement.Implementations;
using Prometheus;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

var cts = new CancellationTokenSource();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(cts);
builder.Services.AddSerilog();
builder.Services.AddDataStore();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseMetricServer();
app.UseHttpsRedirection();
app.UseHttpsRedirection();
app.UseStaticFiles();

DriversStoreEndpointManager.Singleton.RegisterEndpoints(app);
RoutesStoreEndpointManager.Singleton.RegisterEndpoints(app);
VehiclesStoreEndpointManager.Singleton.RegisterEndpoints(app);
app.MapGet("/", async (HttpContext context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync(Path.Combine(app.Environment.WebRootPath, "index.html"));
});

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var exDb = scope.ServiceProvider.GetRequiredService<ICoreContext>();

    exDb.Database.EnsureCreated();
}

app.Run();