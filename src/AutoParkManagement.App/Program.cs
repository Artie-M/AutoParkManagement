using System.Reflection;
using AutoParkManagement.Common;
using AutoParkManagement.Common.Database;
using AutoParkManagement.EndpointManagers;
using AutoParkManagement.Implementations;
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

app.UseHttpsRedirection();

DriverStoreEndpointManager.Singleton.RegisterEndpoints(app);
app.MapGet("/", () => "Hello World!");

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var exDb = scope.ServiceProvider.GetRequiredService<ICoreContext>();

    exDb.Database.EnsureCreated();
}

app.Run();