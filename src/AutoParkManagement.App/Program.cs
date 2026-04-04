using System.Reflection;
using AutoParkManagement.Common;
using AutoParkManagement.Common.Auth;
using AutoParkManagement.Common.Database;
using AutoParkManagement.EndpointManagers;
using AutoParkManagement.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
builder.Services.AddJwtBearerAuth();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMetricServer();
app.UseHttpsRedirection();

app.UseStaticFiles();

DriversStoreEndpointManager.Singleton.RegisterEndpoints(app);
RoutesStoreEndpointManager.Singleton.RegisterEndpoints(app);
VehiclesStoreEndpointManager.Singleton.RegisterEndpoints(app);
AuthEndpointManager.Singleton.RegisterEndpoints(app);

app.MapGet("/", async (HttpContext context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync(Path.Combine(app.Environment.WebRootPath, "index.html"));
});

app.MapGet("/login", async (HttpContext context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync(Path.Combine(app.Environment.WebRootPath, "auth", "login.html"));
});

app.MapGet("/panel", async (HttpContext context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync(Path.Combine(app.Environment.WebRootPath, "admin", "panel.html"));
});

var isDevelopment = app.Environment.IsDevelopment();

if (isDevelopment)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ICoreContext>();

    db.Database.EnsureCreated();
    Log.Logger.Information("Trying to find admin user");
    var admin = await db.Users.FirstOrDefaultAsync(x => x.Name == "admin");
    if (admin == null)
    {
        Log.Logger.Information("Admin user not found");
        var validator = scope.ServiceProvider.GetRequiredService<IUserValidator>();
        await validator.PostUserAsync(db, "admin", "admin");
    }
}

app.Run();
