using AutoParkManagement.Common.Database;
using AutoParkManagement.Common.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace AutoParkManagement.EndpointManagers;

public class VehiclesStoreEndpointManager
{
    public static VehiclesStoreEndpointManager Singleton { get; private set; }

    static VehiclesStoreEndpointManager()
    {
        Singleton = new VehiclesStoreEndpointManager();
    }
    
    private WebApplication _app;
    
    public void RegisterEndpoints(WebApplication app)
    {
        _app = app;
        app.MapGet("api/vehicles/", GetAllVehiclesAsync);
        app.MapGet("api/vehicles/{id:min(0)}", GetVehicleAsync);
        app.MapDelete("api/vehicles/{id:min(0)}", DeleteVehicleAsync);
        app.MapPut("api/vehicles/{id:min(0)}", PutVehicleAsync);
        app.MapPost("api/vehicles/", PostVehicleAsync);
    }

    private async Task GetAllVehiclesAsync(HttpContext httpContext)
    {
        using var scope = _app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ICoreContext>();
        httpContext.Response.ContentType = "application/json; charset=utf-8";
        httpContext.Response.Headers.ContentLanguage = "ru-RU";
        
        httpContext.Response.StatusCode = 200;
        await httpContext.Response.WriteAsJsonAsync(await db.Vehicles.ToListAsync());
    }
    
    private async Task GetVehicleAsync(uint id, HttpContext httpContext)
    {
        using var scope = _app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ICoreContext>();
        var driver = await db.Vehicles.FindAsync(id);
        
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
    
    [Authorize(Roles = "Admin")] 
    private async Task DeleteVehicleAsync(uint id, HttpContext httpContext)
    {
        using var scope = _app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ICoreContext>();
        
        var vehicle = await db.Vehicles.FindAsync(id);
        if (vehicle == null)
        {
            httpContext.Response.StatusCode = 404;
            return;
        }

        db.Vehicles.Remove(vehicle);
        await db.SaveChangesAsync();
        httpContext.Response.StatusCode = 204; // No Content
    }

    [Authorize(Roles = "Admin")] 
    private async Task PutVehicleAsync(uint id, HttpContext httpContext)
    {
        using var scope = _app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ICoreContext>();
        
        var updatedVehicle = await httpContext.Request.ReadFromJsonAsync<VehicleEntity>();
        if (updatedVehicle == null || id != updatedVehicle.Id)
        {
            httpContext.Response.StatusCode = 400; // Bad Request
            return;
        }

        var existingVehicle = await db.Vehicles.FindAsync(id);
        if (existingVehicle == null)
        {
            httpContext.Response.StatusCode = 404;
            return;
        }

        existingVehicle.Name = updatedVehicle.Name;
        existingVehicle.CarNumber = updatedVehicle.CarNumber;

        await db.SaveChangesAsync();
        httpContext.Response.StatusCode = 204;
    }

    [Authorize(Roles = "Admin")] 
    private async Task PostVehicleAsync(HttpContext httpContext)
    {
        using var scope = _app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ICoreContext>();
        
        var newVehicle = await httpContext.Request.ReadFromJsonAsync<VehicleEntity>();
        if (newVehicle == null)
        {
            httpContext.Response.StatusCode = 400; // Bad Request
            return;
        }

        await db.Vehicles.AddAsync(newVehicle);
        await db.SaveChangesAsync();
        
        httpContext.Response.StatusCode = 201; // Created
        await httpContext.Response.WriteAsJsonAsync(newVehicle);
    }
}