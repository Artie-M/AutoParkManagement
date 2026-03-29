using System.Text.Json;
using AutoParkManagement.Common.Database;
using AutoParkManagement.Common.Entities;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;

namespace AutoParkManagement.EndpointManagers;

public class DriversStoreEndpointManager
{
    public static DriversStoreEndpointManager Singleton { get; private set; }

    private WebApplication _app;
    
    static DriversStoreEndpointManager()
    {
        Singleton = new DriversStoreEndpointManager();
    }
    
    public void RegisterEndpoints(WebApplication app)
    {
        _app = app;
        app.MapGet("api/drivers/", GetAllDriversAsync);
        app.MapGet("api/drivers/{id:min(0)}", GetDriverAsync);
        app.MapDelete("api/drivers/{id:min(0)}", DeleteDriverAsync);
        app.MapPut("api/drivers/{id:min(0)}", PutDriverAsync);
        app.MapPost("api/drivers/", PostDriverAsync);
    }

    private async Task GetAllDriversAsync(HttpContext httpContext)
    {
        var redis = ConnectionMultiplexer.Connect("redis:6379");
        var cache = redis.GetDatabase();
        
        httpContext.Response.ContentType = "application/json; charset=utf-8";
        httpContext.Response.Headers.ContentLanguage = "ru-RU";
        httpContext.Response.StatusCode = 200;
        
        string cacheKey = "drivers_all";
        var cachedData = await cache.StringGetAsync(cacheKey);

        if (cachedData.HasValue)
        {
            // Возвращаем данные из кеша
            await httpContext.Response.WriteAsync(cachedData.ToString());
            return;
        }

        using var scope = _app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ICoreContext>();
        
        // Если в кеше нет, берем из БД
        var drivers = await db.Drivers.ToListAsync();
        var serializedData = JsonSerializer.Serialize(drivers);
        
        // Сохраняем в кеш на 5 минут
        await cache.StringSetAsync(cacheKey, serializedData, TimeSpan.FromMinutes(5));
        
        await httpContext.Response.WriteAsync(serializedData);
    }
    
    private async Task GetDriverAsync(uint id, HttpContext httpContext)
    {
        using var scope = _app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ICoreContext>();
        var driver = await db.Drivers.FindAsync(id);
        
        httpContext.Response.ContentType = "application/json; charset=utf-8";
        httpContext.Response.Headers.ContentLanguage = "ru-RU";
        if (driver == null)
        {
            httpContext.Response.StatusCode = 404;
            await httpContext.Response.WriteAsync("Not Found");
            return;
        }
        
        httpContext.Response.StatusCode = 200;
        await httpContext.Response.WriteAsJsonAsync(driver);
    }
    
    private async Task DeleteDriverAsync(uint id, HttpContext httpContext)
    {
        using var scope = _app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ICoreContext>();
        
        var driver = await db.Drivers.FindAsync(id);
        if (driver == null)
        {
            httpContext.Response.StatusCode = 404;
            return;
        }

        db.Drivers.Remove(driver);
        await db.SaveChangesAsync();
        httpContext.Response.StatusCode = 204; 
    }

    private async Task PutDriverAsync(uint id, HttpContext httpContext)
    {
        using var scope = _app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ICoreContext>();
        
        var updatedDriver = await httpContext.Request.ReadFromJsonAsync<DriverEntity>();
        if (updatedDriver == null || id != updatedDriver.Id)
        {
            httpContext.Response.StatusCode = 400; // Bad Request
            return;
        }

        var existingDriver = await db.Drivers.FindAsync(id);
        if (existingDriver == null)
        {
            httpContext.Response.StatusCode = 404;
            return;
        }

        existingDriver.Name = updatedDriver.Name;
        existingDriver.Rating = updatedDriver.Rating;
        existingDriver.CarId = updatedDriver.CarId;

        await db.SaveChangesAsync();    
        httpContext.Response.StatusCode = 204;
    }

    private async Task PostDriverAsync(HttpContext httpContext)
    {
        using var scope = _app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ICoreContext>();
        
        var newDriver = await httpContext.Request.ReadFromJsonAsync<DriverEntity>();
        if (newDriver == null)
        {
            httpContext.Response.StatusCode = 400; // Bad Request
            return;
        }

        await db.Drivers.AddAsync(newDriver);
        await db.SaveChangesAsync();
        
        httpContext.Response.StatusCode = 201; // Created
        await httpContext.Response.WriteAsJsonAsync(newDriver);
    }
}